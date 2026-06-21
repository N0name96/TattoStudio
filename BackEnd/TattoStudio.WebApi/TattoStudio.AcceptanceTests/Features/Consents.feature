Feature: Flujo Legal - Consentimientos y QR (Fase 4)

  Generación de códigos QR, pasarela pública de firmas temporales y
  persistencia inmutable del consentimiento médico vinculado a una cita.
  REG-04-01: El token embebido en el QR es un JWT simétrico válido por 30 minutos.
  REG-04-02: Un consentimiento ya firmado es inmutable (HTTP 403 en re-firma).
  REG-04-03: El consentimiento debe estar firmado antes del cierre del tatuaje (prerrequisito Fase 5).

  Background:
    Given la API está en ejecución
    And existe un usuario registrado con email "staff@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "staff@tattostudio.com" y password "Admin123!"
    And existe un artista de prueba para citas
    And existe un cliente de prueba para citas
    And existe una cita pendiente el "2099-12-15" a las "10:00" UTC con duración 2 horas

  Scenario: Generar QR de consentimiento para una cita existente
    When envío GET autenticado al QR de la última cita creada
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene una imagen QR en Base64
    And el cuerpo de la respuesta contiene un token de consentimiento no vacío

  Scenario: Generar QR de cita inexistente devuelve 404
    When envío GET autenticado a "/api/appointments/00000000-0000-0000-0000-000000000000/qr"
    Then la respuesta tiene el código HTTP 404

  Scenario: Consultar formulario de consentimiento con token válido
    Given el QR de la última cita ha sido generado y el token fue extraído
    When envío GET anónimo al formulario público del consentimiento
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene el campo "appointmentId"
    And el cuerpo de la respuesta contiene el campo "isSigned"

  Scenario: REG-04-01 Rechazar consulta de formulario con token inválido
    When envío GET anónimo a "/api/public/consents/token.invalido.xxx"
    Then la respuesta tiene el código HTTP 401

  Scenario: REG-04-01 Rechazar firma con token inválido
    When envío POST anónimo para firmar el consentimiento en token "token.invalido.xxx" con la firma:
      | SignatureBase64 | IpAddress   |
      | AAEC/w==        | 192.168.1.1 |
    Then la respuesta tiene el código HTTP 401

  Scenario: Firmar consentimiento exitosamente
    Given el QR de la última cita ha sido generado y el token fue extraído
    When envío POST anónimo para firmar el consentimiento con la firma:
      | SignatureBase64 |
      | AAEC/w==        |
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene isSigned true
    And el cuerpo de la respuesta contiene signedAt no nulo

  Scenario: REG-04-02 Rechazar firma duplicada en consentimiento ya firmado
    Given el QR de la última cita ha sido generado y el token fue extraído
    And el consentimiento ya ha sido firmado con la firma "AAEC/w=="
    When envío POST anónimo para firmar el consentimiento con la firma:
      | SignatureBase64 |
      | AAEC/w==        |
    Then la respuesta tiene el código HTTP 403
    And el cuerpo de error contiene el mensaje "firmado"

  Scenario: REG-04-03 El consentimiento firmado queda persistido y es consultable
    Given el QR de la última cita ha sido generado y el token fue extraído
    And el consentimiento ya ha sido firmado con la firma "AAEC/w=="
    When envío GET anónimo al formulario público del consentimiento
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene isSigned true
    And el cuerpo de la respuesta contiene signedAt no nulo
