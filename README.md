Kloniranje repozitorija
Klonirajte repozitorij na svoje računalo:


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

Za ovaj korak nisam siguran da li je ispravan.

json
Copy code
"Jwt": {
    "Key": "thisIsAStrongAndSecureSecretKey123!@#",
    "Issuer": "yourIssuer",
    "Audience": "yourAudience"
}

Projekt razvio: Eugen Deur
Datum: lipanj 2024.

Ovaj README.md file pruža jasne upute o tome kako postaviti, konfigurirati, koristiti i testirati vaš REST API projekt. Prilagodite putanje i postavke prema vašem specifičnom okruženju i zahtjevima projekta.
