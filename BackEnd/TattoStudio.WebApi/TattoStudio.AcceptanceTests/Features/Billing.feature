Feature: Conectividad Externa — VeriFactu y Google Calendar (Fase 6)

  Generación de cadena criptográfica SHA-256 para el Registro VeriFactu (AEAT) y
  sincronización asíncrona con Google Calendar vía eventos de dominio MediatR.
  REG-06-01: Cada factura almacena el SHA-256 de los datos críticos de la anterior,
             formando una cadena inmutable de libro mayor.
  REG-06-02: Si el envío a AEAT falla, AeataStatus queda en PendienteEnvio (0) y un
             BackgroundService reintenta cada 15 minutos sin bloquear al usuario.
  REG-06-03: La creación de citas dispara un AppointmentCreatedEvent procesado de
             forma asíncrona; el fallo de sincronización nunca revierte la cita.

  Background:
    Given la API está en ejecución
    And existe un usuario registrado con email "admin@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "admin@tattostudio.com" y password "Admin123!"
    And existe un artista de prueba para citas
    And existe un cliente de prueba para citas
    And existe una cita pendiente el "2099-12-15" a las "10:00" UTC con duración 2 horas
    And el consentimiento de la última cita ha sido firmado
    And existe un pago de "200.00" tipo 1 método 0 en la última cita

  # ─── BLOQUE 1: Facturación VeriFactu ────────────────────────────────────────

  Scenario: Emitir primera factura genera número secuencial con hash vacío
    When envío POST autenticado a "/api/billing/invoices" con el último pago
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene el campo "invoiceNumber"
    And el cuerpo de la respuesta contiene el campo "previousInvoiceHash"

  Scenario: REG-06-01 Segunda factura almacena el hash SHA-256 de la primera
    Given existe una factura emitida para el último pago
    And existe un pago de "150.00" tipo 0 método 1 en la última cita
    When envío POST autenticado a "/api/billing/invoices" con el último pago
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene previousInvoiceHash no vacío

  Scenario: Consultar estado VeriFactu de factura existente
    Given existe una factura emitida para el último pago
    When envío GET autenticado al estado VeriFactu de la última factura
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene el campo "aeataStatus"
    And el cuerpo de la respuesta contiene el campo "invoiceNumber"

  Scenario: Consultar estado VeriFactu de factura inexistente devuelve 404
    When envío GET autenticado a "/api/billing/invoices/00000000-0000-0000-0000-000000000000/verifactu-status"
    Then la respuesta tiene el código HTTP 404

  # ─── BLOQUE 2: Google OAuth ──────────────────────────────────────────────────

  Scenario: Guardar tokens OAuth de Google Calendar
    When envío POST autenticado a "/api/integrations/google-auth" con los tokens:
      | AccessToken | RefreshToken | ExpiresAt            |
      | acc-token   | ref-token    | 2099-12-31T00:00:00Z |
    Then la respuesta tiene el código HTTP 200

  Scenario: REG-06-03 Crear cita no falla cuando Google Calendar no está configurado
    When envío POST autenticado a "/api/appointments" con datos de la cita:
      | DateTime             | DurationHours | DepositAmount |
      | 2099-12-17T10:00:00Z | 2             | 0.00          |
    Then la respuesta tiene el código HTTP 201
