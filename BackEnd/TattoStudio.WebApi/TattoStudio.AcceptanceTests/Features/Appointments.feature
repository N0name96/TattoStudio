Feature: Motor de Citas (Fase 3)

  Gestión del motor transaccional de reservas.
  REG-03-01: No se permiten fechas pasadas (HTTP 400).
  REG-03-02: No se permite solapamiento en la agenda del artista (HTTP 400).
  REG-03-03: Una seña activa HasDeposit y establece Status Confirmada.
  POST y PUT requieren rol Admin (0) o Recepcion (1).

  Background:
    Given la API está en ejecución
    And existe un usuario registrado con email "recep@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "recep@tattostudio.com" y password "Admin123!"
    And existe un artista de prueba para citas
    And existe un cliente de prueba para citas

  Scenario: Crear cita exitosamente sin seña
    When envío POST autenticado a "/api/appointments" con datos de la cita:
      | DateTime             | DurationHours | DepositAmount |
      | 2099-06-15T10:00:00Z | 2             | 0.00          |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: REG-03-01 Rechazar cita con fecha pasada
    When envío POST autenticado a "/api/appointments" con datos de la cita:
      | DateTime             | DurationHours | DepositAmount |
      | 2020-01-01T10:00:00Z | 2             | 0.00          |
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "futuro"

  Scenario: REG-03-02 Rechazar solapamiento de agenda del artista
    Given el artista de prueba tiene una cita el "2099-07-10" a las "10:00" UTC con duración 3 horas
    When envío POST autenticado a "/api/appointments" con datos de la cita:
      | DateTime             | DurationHours | DepositAmount |
      | 2099-07-10T11:00:00Z | 2             | 0.00          |
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "solapamiento"

  Scenario: REG-03-03 Crear cita con seña activa HasDeposit
    When envío POST autenticado a "/api/appointments" con datos de la cita:
      | DateTime             | DurationHours | DepositAmount |
      | 2099-08-20T14:00:00Z | 2             | 150.00        |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene HasDeposit true

  Scenario: Consultar calendario de citas con rango completo
    When envío GET autenticado a "/api/appointments/calendar?from=2099-01-01&to=2099-12-31"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON

  Scenario: Consultar calendario sin filtro de fechas devuelve todas las citas
    When envío GET autenticado a "/api/appointments/calendar"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON

  Scenario: Consultar calendario solo con fecha de inicio
    When envío GET autenticado a "/api/appointments/calendar?from=2099-01-01"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON

  Scenario: Consultar calendario solo con fecha de fin
    When envío GET autenticado a "/api/appointments/calendar?to=2099-12-31"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON

  Scenario: Confirmar seña de una cita pendiente
    Given existe una cita pendiente el "2099-09-05" a las "09:00" UTC con duración 2 horas
    When envío PUT autenticado para confirmar depósito de "200.00" en la última cita creada
    Then la respuesta tiene el código HTTP 200

  Scenario: Obtener cita por ID exitosamente
    Given existe una cita pendiente el "2099-10-10" a las "09:00" UTC con duración 2 horas
    When envío GET autenticado al endpoint de la última cita creada
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: Obtener cita por ID inexistente devuelve 404
    When envío GET autenticado a "/api/appointments/00000000-0000-0000-0000-000000000000"
    Then la respuesta tiene el código HTTP 404

  Scenario: Filtrar calendario por ClientId del cliente de prueba
    Given existe una cita pendiente el "2099-11-01" a las "10:00" UTC con duración 2 horas
    When envío GET autenticado al calendario filtrado por el último cliente creado
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON

  Scenario: Filtrar calendario por ArtistId del artista de prueba
    Given existe una cita pendiente el "2099-11-15" a las "14:00" UTC con duración 2 horas
    When envío GET autenticado al calendario filtrado por el último artista creado
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON
