# LibraryProject

This project uses Angular version 18 which has the following requirements.
Node.js | TypeScript | RxJS
--- | --- | --- |
^18.19.1 \|\| ^20.11.1 \|\| ^22.0.0 | >= 5.40 < 5.5.0  | ^6.5.3 \|\| ^7.4.0 

Also remember to install the latest version of the angular cli. Running the command "npm install -g @angular/cli" should install the latest version.

Next, make sure to install SQL Server Express (select basic installation during startup) and make a note of the name of the server.
Following that, append the following to the appsettings.json file located in the root directory of the LibraryProjectAPI project.
Make sure to replace "<YOUR_SQLEXPRESS_SERVER> with the name of your server.
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost\\<YOUR_SQLEXPRESS_SERVER>; Database=Library; Integrated Security=True;TrustServerCertificate=True"
    },
  // JwtSettings would need to be updated in a production settings.
  "JwtSettings": {
    "SecurityKey": "MyPlaceholderJwtSecurityKey123453121!",
    "Issuer": "MyIssuer",
    "Audience": "https://localhost:4200",
    "ExpirationTimeInMinutes": 30
  },
  "DefaultUser": {
    "DefaultName": "InitialLibrarian",
    "DefaultEmail" :  "libarian@gmail.com",
    "DefaultPassword" : "MySecretPassword123!"
  }
}
```

Running the project is best done by cloning the project from Visual Studio. Right click on the 'LibraryProject' project solution and click on 'Configure Start up'.
Make sure to select "Multiple startup projects", set the action for both projects to 'Start', and have the LibraryProjectAPI project run first. 
![setup](https://github.com/Feliricki/LibraryProject/assets/54556587/33b6dcb3-c7be-4b4d-8570-84252dc56de5)


Assuming no issues, the user should see the following homepage 

![homepage](https://github.com/Feliricki/LibraryProject/assets/54556587/44240ac2-8547-4282-b654-3cc8bd4bf3ac)

The backend and the database is usually at first, so a refresh or two may be required. Logging in and signing up is self explanatory. The backend instantiates a user in the "Librarian" role at startup with the following crendentials 
```
"DefaultUser": {
  "DefaultName": "InitialLibrarian",
  "DefaultEmail" :  "libarian@gmail.com",
  "DefaultPassword" : "MySecretPassword123!"
}
```
However, at present anyone can sign up as librarian.

Certain options are made unavailable to users not logged in. On the homepage the "Add book" tab and corresponding form is only available to user loggin in as a librarian. The frontend will make the option unavailable and the backend 
server will reject users without the "Librarian" role. 

By clicking on the All Books, one can view all of the books. In addition, one can sort the table by clicking on certain column headers and one can filter by title by typing in the search form. Clicking on the book title will 
take you to the "EditComponent" however only librarians can actually edit a book. All other users will see additional information about the book such as the publisher, publication date, etc.
![editComponent](https://github.com/Feliricki/LibraryProject/assets/54556587/8d09bc8a-c112-4984-9ace-6cbda8b08523)

On this page, any logged in user can checkout a book but only librarian can checkin or edit.

In all in, only logged in users are able to make meaningful changes to the database.


