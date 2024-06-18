Kloniranje repozitorija
Klonirajte repozitorij na svoje računalo:

bash
Copy code
git clone https://github.com/eugendeur1/KINGICT.git

Konfiguracija i podešavanje
Baza podataka:
Projekt koristi lokalnu SQL Server bazu podataka. Podesite vezu s bazom u appsettings.json:

json
Copy code
"ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Autentifikacija i autorizacija:
Za testiranje autentifikacije, koristite sljedeće podatke:

json
Copy code
{
    "username": "KINGICT",
    "password": "KINGICT"
}
Kredencijali su definirani u TestUsers.cs klasi.

JWT Token:
JWT postavke za generiranje tokena nalaze se u appsettings.json:

json
Copy code
"Jwt": {
    "Key": "thisIsAStrongAndSecureSecretKey123!@#",
    "Issuer": "yourIssuer",
    "Audience": "yourAudience"
}
Docker upute (opcionalno)
Ako koristite Docker, možete pokrenuti aplikaciju sljedećim koracima:

bash
Copy code
docker build -t webapplication1-image .
docker run -d -p 8080:80 --name webapplication1-container webapplication1-image
Testiranje aplikacije
Za testiranje API-ja, koristite sljedeće endpointe:

Dohvat svih proizvoda:

bash
Copy code
GET /api/products/GetAllProducts
Dohvat proizvoda po ID-u:

bash
Copy code
GET /api/products/{id}
Filtriranje proizvoda:

sql
Copy code
GET /api/products/filter?category=yourCategory&price=yourPrice
Pretraga proizvoda po nazivu:

sql
Copy code
GET /api/products/search?title=yourTitle
Autor
Projekt razvio: Eugen Deur
Datum: lipanj 2024.

Ovaj README.md file pruža jasne upute o tome kako postaviti, konfigurirati, koristiti i testirati vaš REST API projekt. Prilagodite putanje i postavke prema vašem specifičnom okruženju i zahtjevima projekta.
