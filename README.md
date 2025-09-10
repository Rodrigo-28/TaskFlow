[Readme.txt](https://github.com/user-attachments/files/22264456/Readme.txt)
# TaskFlow - API de tareas programadas

Este proyecto es una API REST construida con .NET 8 y basada en principios de **Clean Architecture**. Fue desarrollado como un desafío personal con el objetivo de aprender a fondo el manejo de tareas en segundo plano utilizando la potente librería **Hangfire**.

---

## 🧠 ¿Por qué este proyecto?

En muchos sistemas reales se necesita ejecutar ciertas tareas de forma programada o diferida: envíos de correos, limpieza de logs, generación de reportes, etc. Quise resolver este tipo de problemas de forma profesional, integrando **Hangfire**, una librería muy usada en la industria para ejecutar **jobs en segundo plano** de manera confiable.

Este proyecto es el resultado de ese desafío: construir una API que permita **programar tareas** que se ejecuten automáticamente en el momento adecuado, sin necesidad de intervención humana.

---

## ⚙️ ¿Qué hace el sistema?

- Permite crear tres tipos de tareas:
  - **EmailTask**: envía un mail de confirmación con fechas personalizadas.
  - **PdfReportTask**: genera un resumen en PDF del estado actual del sistema.
  - **DataCleanupTask**: realiza tareas de limpieza de logs y reportes antiguos.
- Estas tareas se pueden:
  - Ejecutar de inmediato.
  - Programar para una hora específica.
  - Repetir siguiendo una expresión CRON.
- Cada ejecución se registra en logs para trazabilidad y depuración.

---

## 🧱 Arquitectura del proyecto

TaskFlow está construido usando una implementación completa de **Clean Architecture**, separando claramente cada responsabilidad:

- **Domain**: Entidades base como `ScheduledTask`, `EmailTask`, `PdfReportTask`, etc.
- **Application**: Lógica de negocio y servicios (`TaskService`, `EmailService`, `PdfService`, validaciones, DTOs, excepciones, etc.).
- **Infrastructure**: Persistencia con Entity Framework, conexión con Mailgun, generación de PDFs, configuración de Hangfire.
- **Web/Controllers**: Capa de presentación (API REST) con Swagger para testing y documentación interactiva.

### 🧱 ¿Por qué elegí Clean Architecture?

- Para mantener el código desacoplado y testable.
- Para poder cambiar fácilmente la infraestructura (por ejemplo, otra librería de email) sin afectar la lógica.
- Para aplicar buenas prácticas y construir un sistema escalable y mantenible a largo plazo.

📚 Más info: [The Clean Architecture – Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## ⏱️ Integración con Hangfire

[Hangfire](https://www.hangfire.io/) es una librería .NET que permite ejecutar métodos de forma diferida, recurrente o inmediata, en **segundo plano**. Internamente, Hangfire utiliza un **job server** que corre en un **hilo o proceso separado**, lo que permite que el flujo principal de la API no se vea afectado.

### Ejecución en segundo plano

Cuando el usuario crea una tarea, se programa con `HangfireJobScheduler`, el cual puede hacer tres cosas:

- `BackgroundJob.Enqueue(...)`: ejecuta lo antes posible (fire-and-forget).
- `BackgroundJob.Schedule(...)`: ejecuta una vez en el futuro.
- `RecurringJob.AddOrUpdate(...)`: ejecuta de forma cíclica según una expresión CRON.

Por ejemplo, una tarea tipo `EmailTask`:

1. Se guarda en la base de datos.
2. Se registra con Hangfire (diferido, inmediato o recurrente).
3. Hangfire ejecuta el método `ExecuteTaskAsync(id)` desde otro hilo.
4. `ExecuteTaskAsync` detecta el tipo de tarea y ejecuta su lógica (enviar mail, generar PDF, limpiar datos).
5. Se registra la ejecución exitosa o fallida en los logs.

Esta separación de hilos es ideal para tareas pesadas o demoradas como envíos de correos o generación de reportes, sin afectar la experiencia del usuario ni saturar el hilo principal del servidor web.

📚 Más info: [Documentación oficial de Hangfire](https://docs.hangfire.io/en/latest/)

---

## 📧 Notificaciones por correo con Mailgun

Se utiliza el servicio [Mailgun](https://www.mailgun.com/) para el envío automático de correos. Está completamente desacoplado mediante un servicio (`EmailService`) que encapsula la lógica de construcción del mensaje y manejo de errores.

La configuración se realiza en `appsettings.json`:

```json
"Mailgun": {
  "ApiKey": "",
  "Domain": ""
}
```

🔗 Documentación: [Mailgun API Docs](https://documentation.mailgun.com/en/latest/)

---

## 🧾 Generación de PDFs

Las tareas `PdfReportTask` generan un archivo PDF que resume el estado actual del sistema y lo guarda localmente. Se utiliza una clase `PdfService` personalizada, sin depender de servicios externos.

---

## 🧹 Limpieza de datos y archivos

El sistema incluye tareas `DataCleanupTask` que ejecutan lógica para borrar archivos de reportes antiguos y logs de ejecución.

Configuración en `appsettings.json`:

```json
"Cleanup": {
  "ReportsFolder": "Reports",
  "DefaultLogRetentionDays": 30,
  "DefaultFileRetentionDays": 7
}
```

Esto asegura que el sistema no se llene con archivos obsoletos o registros innecesarios.

---

## 🛢️ Base de datos utilizada

Se utiliza **PostgreSQL** como sistema de gestión de base de datos. Todos los datos de tareas, logs de ejecución y configuraciones se almacenan ahí.

🔗 Más info: [PostgreSQL](https://www.postgresql.org/)

La conexión se configura desde `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=;Username=;Password="
}
```

---

## 🕓 Formato global de fecha y hora

El sistema utiliza el formato de fecha estándar ISO 8601 con zona horaria explícita. Ejemplo:

```
2025-07-06 22:00:00.000 -0300
```

Esto asegura que las tareas programadas se interpreten correctamente sin errores por diferencias horarias entre servidores y clientes.

---

## 🔍 Ejemplo de flujo completo

1. El usuario crea una tarea `EmailTask` para ejecutarse a las 22:00 del día siguiente.
2. La API valida que `ScheduledTime` sea futuro.
3. Se guarda en la base de datos.
4. Se programa con `HangfireJobScheduler.ScheduleDelayedTask(...)`.
5. Hangfire ejecuta `ExecuteTaskAsync(id)` a la hora indicada.
6. El sistema detecta que es un `EmailTask`, llama a `SendConfirmationEmailAsync`.
7. Se registra el éxito (o falla) en los `TaskExecutionLogs`.

Todo esto ocurre de forma automática, sin necesidad de que nadie esté usando la API en ese momento.

---

## 💡 ¿Dónde se puede aplicar esto?

- Plataformas de reportes automáticos.
- Envío de recordatorios o promociones programadas.
- Limpieza periódica de datos o cachés.
- Generación de backups automáticos.
- Integración con procesos externos que dependen del tiempo.

---

## 🚀 Ideas para mejoras futuras

- Panel web para visualizar, editar y pausar tareas.
- Notificaciones si una tarea falla.
- Integración con Redis para monitoreo más avanzado.
- Soporte multiusuario con autenticación y permisos.
- Reintentos y lógica de recuperación personalizada.

---

## 📚 Tecnologías y librerías utilizadas

- [.NET 8](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Hangfire](https://www.hangfire.io/)
- [Mailgun](https://www.mailgun.com/)
- [Swagger (Swashbuckle)](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [PostgreSQL](https://www.postgresql.org/)

---

## ✍️ Conclusión

Este proyecto fue una excelente oportunidad para dominar el uso de Hangfire y la programación de tareas en segundo plano en .NET. Aprendí a construir flujos asíncronos robustos, desacoplar responsabilidades y diseñar una arquitectura profesional.

Más allá del código, es una demostración de cómo usar herramientas modernas para resolver problemas reales, de forma clara, eficiente y escalable.
