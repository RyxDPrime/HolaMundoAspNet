This simple project is made by the ASP.NET framework, using VS Code with .NET SDK - Version 10.0 LTS
Instalador de .NET SDK - https://dotnet.microsoft.com/es-es/download

Comandos usados para el proyecto:
1. Creacion y ejecucion del proyecto
    dotnet new web - Crea un nuevo proyecto del framework ASP.NET vacio
    dotnet run - Compila y ejecuta la aplicacion
    dotnet watch run - Ejecuta la app y reinicia el servidor automaticamente cuando se guarda un cambio en el codigo

2. Gestion de Paquetes:
    dotnet add package Microsoft.EntityFrameworkCore.Sqlite - Instala el motor para usar bases de datos SQLite.
    dotnet add package Microsoft.EntityFrameworkCore.Design - Instala las herramientas necesarias para manejar la base de datos.

3. Herramientas de Base de Datos (Entity Framework)
    dotnet tool install --global dotnet-ef - Instala la herramienta global de Entity Framework (solo se hace una vez, no es necesario hacerlo cada vez que se crea un proyecto nuevo).
    dotnet ef migrations add <Nombre de la Migracion> - Analiza el codigo y crea un archivo de instraucciones sobre como manejar/modificar la base de datos
    dotnet ef database update - Aplica las instrucciones que se generan con el comando anterior al archivo de la base de datos.