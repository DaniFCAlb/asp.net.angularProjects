Simple WebApi to work with authentication and authorization

issues to solve:
    - can make a post request to register and login a user but can't GET the user, status 401
       1 - POST request login to get the token, but when make the GET with token, status 401
            [23:12:47 INF] Request starting HTTP/1.1 GET http://localhost:5157/api/WeatherForeCast application/json 61
            [23:12:47 DBG] 1 candidate(s) found for the request path '/api/WeatherForeCast'
            [23:12:47 DBG] Endpoint 'AuthenticApp.Auth.Controllers.WeatherForecastController.Get (AuthenticApp)' with route pattern 'api/WeatherForecast' is valid for the request path '/api/WeatherForeCast'
            [23:12:47 DBG] Request matched endpoint 'AuthenticApp.Auth.Controllers.WeatherForecastController.Get (AuthenticApp)'
            [23:12:47 DBG] Static files was skipped as the request already matched an endpoint.
            [23:12:47 INF] Bearer was not authenticated. Failure message: No SecurityTokenValidator available for token.
            [23:12:47 INF] Authorization failed. These requirements were not met:
            DenyAnonymousAuthorizationRequirement: Requires an authenticated user.
            [23:12:47 INF] AuthenticationScheme: Bearer was challenged.
            [23:12:47 DBG] Connection id "0HMOFL1HMQOBU" completed keep alive response.
    

Requests: 
    - POST register: 
        - http://localhost:5157/api/Authenticate/register
        {
            "username": "User",
            "email": "user@pt.com",
            "password": "Password@123"
        }
    - POST login :
        - http://localhost:5157/api/Authenticate/login
        {
            "username": "User",
            "password": "Password@123"
        }

    -GET http://localhost:5157/api/WeatherForeCast -- 1- not Working

    - POST register-admin: 
        - http://localhost:5157/api/Authenticate/register-admin
        {
            "username" :"EU-admin",
            "email" : "eu-damin@mail.com",
            "password" : "Password@123"
        }

    - POST login:
        - http://localhost:5157/api/Authenticate/login
        {
            "username" :"EU-admin",
            "password" : "Password@123"
        }
    -GET http://localhost:5157/api/WeatherForeCast -- 1- not Working
    
    
    to make this work on your local machine:
    - delete folder migrations
    - on appsettings.json change the connectionString to your connectioString of you DB
    - on terminal run - dotnet ef migrations add InitialCreate
                        - dotnet ef database update
                        - dotnet run 
                        
