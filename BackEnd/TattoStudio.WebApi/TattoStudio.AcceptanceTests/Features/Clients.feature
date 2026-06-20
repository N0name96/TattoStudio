Feature: Gestión de Clientes (Fase 2)

  Los endpoints de clientes requieren autenticación JWT con rol Admin o Recepcion.

  Background:
    Given la API está en ejecución
    And existe un usuario registrado con email "admin@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "admin@tattostudio.com" y password "Admin123!"

  Scenario: Crear cliente exitosamente
    When envío POST autenticado a "/api/clients" con datos del cliente:
      | Name         | Phone     | Email                 | BirthDate  | MedicalNotes     |
      | Pedro García | 612345678 | pedro@tattostudio.com | 2000-01-15 | Alérgico a látex |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: Crear cliente sin notas médicas exitosamente
    When envío POST autenticado a "/api/clients" con datos del cliente:
      | Name       | Phone     | Email                   | BirthDate  |
      | Ana Martín | 699001122 | ana@tattostudio.com     | 2005-07-10 |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: Obtener ficha de cliente existente
    Given existe un cliente con email "ficha@tattostudio.com" y fecha de nacimiento "1995-03-20"
    When envío GET autenticado al endpoint del último cliente creado
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene el email "ficha@tattostudio.com"

  Scenario: Obtener cliente inexistente devuelve 404
    When envío GET autenticado a "/api/clients/00000000-0000-0000-0000-000000000000"
    Then la respuesta tiene el código HTTP 404
