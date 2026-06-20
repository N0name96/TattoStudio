Feature: Gestión de Artistas (Fase 2)

  REG-02-02: La comisión del artista debe estar entre 0.00 y 100.00.
  La lista de artistas es pública; la creación requiere rol Admin.

  Background:
    Given la API está en ejecución

  Scenario: Crear artista exitosamente
    Given existe un usuario registrado con email "admin@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "admin@tattostudio.com" y password "Admin123!"
    When envío POST autenticado a "/api/artists" con datos del artista:
      | Name         | Specialty | CommissionPercentage |
      | Marta Torres | Realismo  | 50.00                |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: REG-02-02 Rechazar comisión superior a 100
    Given existe un usuario registrado con email "admin2@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "admin2@tattostudio.com" y password "Admin123!"
    When envío POST autenticado a "/api/artists" con datos del artista:
      | Name      | Specialty | CommissionPercentage |
      | Artista X | Tribal    | 150.00               |
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "comisión"

  Scenario: REG-02-02 Rechazar comisión negativa
    Given existe un usuario registrado con email "admin3@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "admin3@tattostudio.com" y password "Admin123!"
    When envío POST autenticado a "/api/artists" con datos del artista:
      | Name      | Specialty | CommissionPercentage |
      | Artista Y | Acuarela  | -5.00                |
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "comisión"

  Scenario: Listar artistas es accesible públicamente
    When envío GET a "/api/artists"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta es una lista JSON
