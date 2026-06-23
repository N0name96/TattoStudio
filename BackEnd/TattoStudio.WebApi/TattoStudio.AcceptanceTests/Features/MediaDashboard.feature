Feature: Multimedia, Notificaciones y Dashboard (Fase 7)

  Gestión de archivos multimedia por cita (Supabase Storage), auditoría de
  notificaciones asíncronas y métricas analíticas del Dashboard de administración.
  REG-07-01: Solo image/jpeg, image/png e image/webp; tamaño máximo 5 MB por archivo.
  REG-07-02: Las notificaciones de recordatorio se procesan en BackgroundService;
             el fallo actualiza NotificationLogs.Status = 2 sin interrumpir la cita.
  REG-07-03: Los endpoints del Dashboard usan AsNoTracking y agrupaciones LINQ
             ejecutadas directamente en el servidor de Supabase.

  Background:
    Given la API está en ejecución
    And existe un usuario registrado con email "admin@tattostudio.com" y password "Admin123!" y rol 0
    And estoy autenticado con email "admin@tattostudio.com" y password "Admin123!"
    And existe un artista de prueba para citas
    And existe un cliente de prueba para citas
    And existe una cita pendiente el "2099-12-15" a las "10:00" UTC con duración 2 horas

  # ─── BLOQUE 1: Multimedia (REG-07-01) ───────────────────────────────────────

  Scenario: Subir imagen JPEG válida a una cita
    When envío POST autenticado con imagen "sketch.jpg" tipo "image/jpeg" mediaType 1 a la última cita
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene el campo "storageUrl"
    And el cuerpo de la respuesta contiene el campo "id"

  Scenario: Subir imagen PNG válida a una cita
    When envío POST autenticado con imagen "result.png" tipo "image/png" mediaType 2 a la última cita
    Then la respuesta tiene el código HTTP 201
    And el cuerpo de la respuesta contiene el campo "storageUrl"

  Scenario: REG-07-01 Rechazar archivo con tipo MIME inválido
    When envío POST autenticado con imagen "doc.pdf" tipo "application/pdf" mediaType 0 a la última cita
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "formato"

  Scenario: REG-07-01 Rechazar imagen mayor de 5 MB
    When envío POST autenticado con imagen de 6 MB tipo "image/jpeg" mediaType 0 a la última cita
    Then la respuesta tiene el código HTTP 400
    And el cuerpo de error contiene el mensaje "5"

  Scenario: Eliminar media existente de una cita devuelve 204
    Given existe una imagen subida a la última cita con mediaType 0
    When envío DELETE autenticado al último media de la última cita
    Then la respuesta tiene el código HTTP 204

  Scenario: Eliminar media inexistente devuelve 404
    When envío DELETE autenticado a "/api/appointments/00000000-0000-0000-0000-000000000000/media/00000000-0000-0000-0000-000000000001"
    Then la respuesta tiene el código HTTP 404

  # ─── BLOQUE 2: Dashboard (REG-07-03) ────────────────────────────────────────

  Scenario: REG-07-03 Consultar resumen del Dashboard devuelve métricas consolidadas
    When envío GET autenticado a "/api/dashboard/summary?year=2099&month=12"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene el campo "totalRevenue"
    And el cuerpo de la respuesta contiene el campo "totalAppointments"
    And el cuerpo de la respuesta contiene el campo "totalCommissionsPaid"
    And el cuerpo de la respuesta contiene el campo "lowStockAlertsCount"

  Scenario: Dashboard de mes sin actividad devuelve ceros
    When envío GET autenticado a "/api/dashboard/summary?year=2050&month=1"
    Then la respuesta tiene el código HTTP 200
    And el cuerpo de la respuesta contiene totalAppointments igual a 0
