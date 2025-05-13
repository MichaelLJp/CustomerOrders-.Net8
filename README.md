=====================================================================
                           CUSTOMERORDERS
=====================================================================

CustomerOrders es una aplicación diseñada para gestionar clientes y 
sus órdenes, implementada siguiendo los principios de Clean Architecture. 
El proyecto está estructurado en capas claramente definidas y en 
múltiples proyectos (Aplicación, Dominio, Infraestructura, Presentación y 
Pruebas), lo que facilita la mantenibilidad, el escalado y la cobertura 
de pruebas unitarias e integración.

*** Este proyecto fue creado en .NET 8, aprovechando las últimas 
    innovaciones y mejoras de rendimiento de la plataforma. ***

---------------------------------------------------------------------
TABLA DE CONTENIDOS
---------------------------------------------------------------------
1. Introducción
2. Requisitos
3. Instalación
4. Ejecución
5. Estructura del Proyecto
6. Pruebas
7. Arquitectura y Buenas Prácticas
8. Contribuciones
9. Licencia
10. Contacto

---------------------------------------------------------------------
1. INTRODUCCIÓN
---------------------------------------------------------------------
CustomerOrders es una solución para administrar clientes y sus órdenes, 
permitiendo la creación, actualización, eliminación y consulta de datos. 
Se utiliza Entity Framework Core para el manejo de la persistencia y se 
incorpora una base de datos en memoria (InMemoryDatabase) para realizar 
pruebas unitarias e integrales sin depender de una base de datos real.

---------------------------------------------------------------------
2. REQUISITOS
---------------------------------------------------------------------
- .NET 8 SDK: Descarga la última versión en https://dotnet.microsoft.com/download
- Un editor de código, por ejemplo, Visual Studio Code o Visual Studio.
- Git para clonar el repositorio.

---------------------------------------------------------------------
3. INSTALACIÓN
---------------------------------------------------------------------
Paso 1. Clonar el repositorio:
   git clone https://github.com/MichaelLJp/CustomerOrders-.Net8.git
   cd CustomerOrders-.Net8

Paso 2. Restaurar las dependencias:
   dotnet restore

Paso 3. Compilar el proyecto:
   dotnet build

---------------------------------------------------------------------
4. EJECUCIÓN
---------------------------------------------------------------------
Para ejecutar la aplicación, ubícate en el directorio del proyecto principal 
y ejecuta:
   dotnet run --project CustomerOrders.Presentation

Esto iniciará el servidor (por ejemplo, en el caso de una API) y mostrará 
los endpoints disponibles.

---------------------------------------------------------------------
5. ESTRUCTURA DEL PROYECTO
---------------------------------------------------------------------
La solución se organiza en los siguientes proyectos:

1. CustomerOrders.Application
   - Dtos                
         -> Independencia de Data Transfer Objects para la comunicación entre capas.
   - Interfaces        
         -> Definición de contratos tanto para la comunicación entre capas como para la 
            declaración de contratos de servicios y validaciones propias de la lógica de negocio.
   - Mappings            
         -> Configuración de AutoMapper para la transformación entre DTOs y entidades.
   - Services          
         -> Implementaciones de la lógica de negocio, basadas en las interface

2. CustomerOrders.Domain
  - Entities
         -> Definición de las entidades del dominio (por ejemplo, Customer, Order, etc.) que representan los modelos de negocio.  
  - Interfaces  
         -> Contratos de repositorios (por ejemplo, IRepository.cs).



3. CustomerOrders.Infrastructure
    - Data  
         -> Contiene el DbContext y la configuración de la conexión a la base de datos.  
   - Migrations  
         -> Scripts y configuraciones generadas para administrar las migraciones de la base de datos.  
   - Repositories  
         -> Implementación de repositorios, incluyendo el repositorio genérico que simplifica las operaciones comunes.



4. CustomerOrders.Presentation
   - Connected Services  
         -> Servicios conectados (por ejemplo, archivos utilizados para pruebas de API).  
   - Controllers  
         -> Manejan las peticiones HTTP y delegan la lógica de negocio a los servicios de la capa Application.  
   - appsettings.json  
         -> Archivo de configuración de la aplicación que contiene ajustes como cadenas de conexión, configuración del logging y otros parámetros necesarios para la ejecución de la aplicación.  
   - CustomerOrders.Presentation.http  
         -> Archivo para pruebas de endpoints con herramientas como REST Client
   - Program.cs        -> Configuración del contenedor de inyección de dependencias.

5. CustomerOrders.Tests
   - Controllers  
         -> Pruebas unitarias y de integración para los controladores.  
   - Services  
         -> Pruebas para la lógica de negocio y validaciones en los servicios.  
   - Repositories  
         -> Pruebas para validar las operaciones de acceso a datos y repositorios.


Esta organización sigue el enfoque Clean Architecture, separando las 
responsabilidades en proyectos específicos para la lógica de aplicación, 
el dominio, la infraestructura, la presentación y las pruebas, lo que favorece 
el mantenimiento y la escalabilidad del sistema.

---------------------------------------------------------------------
6. PRUEBAS
---------------------------------------------------------------------
El proyecto incluye pruebas unitarias utilizando frameworks como xUnit, 
Moq y FluentAssertions. Las pruebas cubren:

- Controladores: Validación de endpoints y respuestas HTTP.
- Servicios: Lógica de negocio, validaciones y manejo de excepciones.
- Repositorios: Operaciones de acceso a datos e integridad, utilizando 
  EF Core InMemoryDatabase para simular la base de datos.

Para ejecutar las pruebas, utiliza:
   dotnet test

---------------------------------------------------------------------
7. ARQUITECTURA Y BUENAS PRÁCTICAS
---------------------------------------------------------------------
- **Separación de responsabilidades:**  
  Cada capa se encarga de una responsabilidad específica, permitiendo 
  modificaciones en la infraestructura o presentación sin afectar la lógica 
  del dominio.  
  
- **Independencia de DTOs:**  
  Los Data Transfer Objects (DTOs) se usan para desacoplar la comunicación 
  entre la capa de presentación y la lógica de negocio, asegurando que los 
  cambios internos no afecten la API pública.

- **Uso de AutoMapper:**  
  Se implementa AutoMapper para transformar de forma sencilla y 
  consistente los DTOs en entidades y viceversa, reduciendo el código 
  repetitivo y facilitando la mantenibilidad.

- **Inyección de Dependencias:**  
  El proyecto utiliza inyección de dependencias para resolver 
  automáticamente las dependencias de cada componente, permitiendo una 
  arquitectura flexible y fácilmente testeable, tal como se configura en 
  el `Program.cs` de CustomerOrders.Presentation.

- **Interfaces y Repositorio Genérico:**  
  El uso de interfaces en la capa de dominio (como IRepository.cs) y la 
  implementación de un repositorio genérico en la infraestructura facilitan la 
  extensión y reutilización del código, permitiendo intercambiar implementaciones 
  sin afectar el resto de la aplicación.

- **Cobertura completa de pruebas:**  
  Se incluyen pruebas unitarias y de integración para validar el funcionamiento 
  y los escenarios extremos (por ejemplo, datos inválidos o inconsistencias).

- **Uso de InMemoryDatabase para pruebas:**  
  Esto evita depender de bases de datos reales, acelerando y simplificando 
  la ejecución de tests.

---------------------------------------------------------------------
8. CONTRIBUCIONES
---------------------------------------------------------------------
¡Las contribuciones son bienvenidas! Para aportar mejoras o corregir errores:

1. Haz un fork del repositorio.
2. Crea una rama para tu nueva funcionalidad o corrección 
   (por ejemplo: git checkout -b feature/nueva-caracteristica).
3. Realiza tus cambios y escribe pruebas que validen la funcionalidad.
4. Envía un pull request describiendo tus modificaciones.

---------------------------------------------------------------------
9. LICENCIA
---------------------------------------------------------------------
Este proyecto se distribuye bajo la Licencia MIT. Puedes usar, modificar 
y distribuir el código de acuerdo a los términos establecidos en la licencia.

---------------------------------------------------------------------
10. CONTACTO
---------------------------------------------------------------------
Si tienes dudas o sugerencias, contacta a:
   - Nombre: Michael
   - Email: michael@example.com (o el email que prefieras compartir)

---------------------------------------------------------------------
¡Gracias por usar CustomerOrders!
Esperamos que la robusta estructura, el uso de buenas prácticas 
y la completa cobertura de pruebas hagan tu desarrollo y mantenimiento 
más eficientes.