Feature: Autenticación y Registro de Usuarios

  Las reglas REG-01-01, REG-01-02 y REG-01-03 definen el ciclo completo de registro,
  hasheo de contraseñas y emisión de tokens JWT para el staff del estudio de tatuajes.

  Background:
    Given la API está en ejecución

  Scenario: Registro exitoso de un nuevo usuario
    Given no existe un usuario con el email "nuevo@tattostudio.com"
    When envío POST a "/api/auth/register" con el cuerpo:
      | Email                  | Password       | Name       | Role |
      | nuevo@tattostudio.com  | SecurePass123! | Juan Pérez | 1    |
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene un Id de tipo Guid válido

  Scenario: REG-01-01 Registro rechazado por email duplicado
    Given existe un usuario registrado con email "duplicado@tattostudio.com" y password "Pass123!"
    When envío POST a "/api/auth/register" con el cuerpo:
      | Email                     | Password  | Name      | Role |
      | duplicado@tattostudio.com | OtroPass! | Ana López | 1    |
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "Email ya registrado"

  Scenario: REG-01-02 Login exitoso con credenciales válidas
    Given existe un usuario registrado con email "login@tattostudio.com" y password "ValidPass456!"
    When envío POST a "/api/auth/login" con el cuerpo:
      | Email                  | Password      |
      | login@tattostudio.com  | ValidPass456! |
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene un token JWT no vacío

  Scenario: REG-01-02 Login rechazado con credenciales inválidas
    Given existe un usuario registrado con email "usuario@tattostudio.com" y password "CorrectPass!"
    When envío POST a "/api/auth/login" con el cuerpo:
      | Email                    | Password        |
      | usuario@tattostudio.com  | ContrasenaMal!  |
    Then la respuesta tiene el código HTTP 401
    And el cuerpo de error contiene el mensaje "Credenciales inválidas"

  Scenario: REG-01-03 El token JWT incluye los claims obligatorios y expira en 8 horas
    Given existe un usuario registrado con email "admin@tattostudio.com" y password "AdminPass789!" y rol 0
    When envío POST a "/api/auth/login" con el cuerpo:
      | Email                  | Password      |
      | admin@tattostudio.com  | AdminPass789! |
    Then la respuesta tiene el código HTTP 200
    And el token JWT contiene el claim "sub"
    And el token JWT contiene el claim "email" con valor "admin@tattostudio.com"
    And el token JWT contiene el claim "role" con valor "0"
    And el token JWT expira exactamente en 8 horas desde su emisión
