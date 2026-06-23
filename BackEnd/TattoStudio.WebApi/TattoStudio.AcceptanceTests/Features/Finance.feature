Feature: Finanzas y Logística — Pagos, Cierre y Stock (Fase 5)

  Gestión de cobros parciales y totales, cierre atómico de citas con registro
  automático de comisión al artista, descuento de inventario y alertas de stock bajo.
  REG-05-01: El cierre es transaccional — verifica consentimiento firmado,
             registra comisión (Total Pagos * CommissionPercentage / 100),
             resta stock y marca la cita como Completada. Todo o nada.
  REG-05-02: Si CurrentQuantity <= MinThreshold tras descontar stock,
             la respuesta incluye RequiresRestock: true.

  Background:
    Given la API está en ejecución
    And existe un usuario registrado con email "finance@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "finance@tattostudio.com" y password "Admin123!"
    And existe un artista de prueba para citas
    And existe un cliente de prueba para citas
    And existe una cita pendiente el "2099-12-15" a las "10:00" UTC con duración 2 horas
    And el consentimiento de la última cita ha sido firmado
    And existe un ítem de stock con nombre "Tinta Negra" cantidad "10" y umbral mínimo "3"

  # ─────────────────────────────────────
  # BLOQUE 1: Pagos
  # ─────────────────────────────────────

  Scenario: Registrar una seña en la cita
    When envío POST autenticado a "/api/payments" con los datos de pago:
      | Amount | Type | Method |
      | 100.00 | 0    | 0      |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: Registrar el pago final de la cita
    When envío POST autenticado a "/api/payments" con los datos de pago:
      | Amount | Type | Method |
      | 350.00 | 1    | 2      |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: Rechazar pago con importe cero
    When envío POST autenticado a "/api/payments" con los datos de pago:
      | Amount | Type | Method |
      | 0.00   | 0    | 0      |
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "positivo"

  Scenario: Rechazar pago en cita inexistente devuelve 404
    When envío POST autenticado a "/api/payments" en la cita "00000000-0000-0000-0000-000000000000" con los datos:
      | Amount | Type | Method |
      | 50.00  | 0    | 0      |
    Then la respuesta tiene el código HTTP 404

  # ─────────────────────────────────────
  # BLOQUE 2: Cierre transaccional (REG-05-01)
  # ─────────────────────────────────────

  Scenario: REG-05-01 Completar cita ejecuta flujo transaccional completo
    When envío POST autenticado para completar la última cita usando el ítem "Tinta Negra" con cantidad 1
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene el estado "Completada"
    And el cuerpo de la respuesta contiene el campo "commissionAmount"
    And el cuerpo de la respuesta contiene requiresRestock false

  Scenario: REG-05-01 Rechazar cierre si el consentimiento no está firmado
    Given existe una cita pendiente el "2099-12-20" a las "14:00" UTC con duración 2 horas
    When envío POST autenticado para completar la última cita usando el ítem "Tinta Negra" con cantidad 1
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "consentimiento"

  Scenario: Rechazar cierre de cita inexistente devuelve 404
    When envío POST autenticado a "/api/appointments/00000000-0000-0000-0000-000000000000/complete" con items:
      | StockItemName | Quantity |
      | Tinta Negra   | 1        |
    Then la respuesta tiene el código HTTP 404

  # ─────────────────────────────────────
  # BLOQUE 3: Alerta de stock (REG-05-02)
  # ─────────────────────────────────────

  Scenario: REG-05-02 Alerta de stock bajo al cerrar cita
    Given existe un ítem de stock con nombre "Tinta Roja" cantidad "2" y umbral mínimo "3"
    And existe una cita pendiente el "2099-12-16" a las "11:00" UTC con duración 2 horas
    And el consentimiento de la última cita ha sido firmado
    When envío POST autenticado para completar la última cita usando el ítem "Tinta Roja" con cantidad 2
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene requiresRestock true

  Scenario: Consultar ítems con stock bajo
    Given existe un ítem de stock con nombre "Agujas 7M" cantidad "1" y umbral mínimo "5"
    When envío GET autenticado a "/api/stock/low-inventory"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON
    And la lista contiene al menos un ítem con requiresRestock true

  Scenario: Stock por encima del umbral no aparece en alertas
    When envío GET autenticado a "/api/stock/low-inventory"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON
    And la lista no contiene el ítem "Tinta Negra"
