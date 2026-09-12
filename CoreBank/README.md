# 🏦 CoreBank - Bireysel İnternet Bankacılığı Uygulaması

CoreBank; modern web standartlarına uygun, güvenli, ölçeklenebilir ve RESTful mimari prensipleriyle geliştirilmiş bir bireysel internet bankacılığı simülasyonudur.

---

## 🚀 Teknolojiler ve Mimari

### Backend
* **Framework:** ASP.NET Core Web API (.NET)
* **Veritabanı & ORM:** Microsoft SQL Server, Entity Framework Core (Code-First)
* **Güvenlik & Yetkilendirme:** JWT (JSON Web Tokens), BCrypt.Net şifreleme
* **Mimari Yapı:** Service-Repository Pattern, DTO deseni, Global Exception Handling Middleware

### Frontend
* **Arayüz & Stil:** HTML5, CSS3, Bootstrap 5, Bootstrap Icons
* **İstemci Mantığı:** Modern JavaScript (Fetch API, Async/Await, DOM Manipülasyonu)
* **Oturum Yönetimi:** LocalStorage tabanlı güvenli token saklama

---

## ✨ Temel Özellikler

* **Kullanıcı İşlemleri:** Güvenli kayıt ve giriş akışı, BCrypt ile şifrelenmiş parola saklama, JWT tabanlı oturum yönetimi.
* **Otomatik Hesap Açılışı:** Kayıt esnasında Türkiye standartlarına uygun (TR + 24 hane rakam) benzersiz IBAN ve başlangıç bakiyesi tanımlama.
* **Para Transferi:** Kullanıcılar arası anlık bakiye aktarımı, bakiye ve IBAN doğrulama kontrolleri, atomik işlem yönetimi.
* **Hesap Hareketleri:** Gerçek zamanlı gelen/giden transfer geçmişi listelemesi ve işlem detayları.

---

## 🛠️ Kurulum ve Çalıştırma

### 1. Projeyi Klonlayın

git clone [https://github.com/kullanici-adiniz/CoreBank.git](https://github.com/kullanici-adiniz/CoreBank.git)
cd CoreBank/CoreBank.API

2. Veritabanı Ayarları
appsettings.json dosyasındaki SQL Server bağlantı dizesini (ConnectionStrings) ve JWT anahtarını güncelleyin:

JSON

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CoreBankDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

3. Migration ve Veritabanı Güncellemesi

dotnet ef database update

4. API'yi Başlatın

dotnet run

5. Frontend Arayüzünü Açın

auth.html veya index.html dosyasını tarayıcınızda açarak uygulamayı kullanmaya başlayabilirsiniz.


---

# 🏦 CoreBank - Personal Internet Banking Application

CoreBank is a personal internet banking simulation built with modern web standards, adhering to secure, scalable, and RESTful architecture principles.

---

## 🚀 Technologies & Architecture

### Backend
* **Framework:** ASP.NET Core Web API (.NET)
* **Database & ORM:** Microsoft SQL Server, Entity Framework Core (Code-First)
* **Security & Authentication:** JWT (JSON Web Tokens), BCrypt.Net password hashing
* **Architecture:** Service-Repository Pattern, DTO Pattern, Global Exception Handling Middleware

### Frontend
* **UI & Styling:** HTML5, CSS3, Bootstrap 5, Bootstrap Icons
* **Client Logic:** Modern JavaScript (Fetch API, Async/Await, DOM Manipulation)
* **Session Management:** LocalStorage-based secure token storage

---

## ✨ Key Features

* **User Management:** Secure registration and login flow, BCrypt password hashing, JWT-based session authorization.
* **Automated Account Creation:** Generates a unique IBAN conforming to Turkish banking standards (TR + 24 digits) with an initial seeded balance upon registration.
* **Money Transfers:** Instant fund transfers between accounts with balance verification, recipient validation, and atomic database transactions.
* **Account Activities:** Real-time transaction history display for incoming and outgoing transfers with formatted local timestamps.

---

## 🛠️ Setup & Installation

### 1. Clone the Repository

git clone [https://github.com/kullanici-adiniz/CoreBank.git](https://github.com/kullanici-adiniz/CoreBank.git)
cd CoreBank/CoreBank.API

2. Database Configuration
Update the SQL Server connection string and JWT secret in appsettings.json:

JSON

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CoreBankDb;Trusted_Connection=True;TrustServerCertificate=True;"
}

3. Run Migrations & Update Database

dotnet ef database update

4. Run the API

dotnet run

5. Launch Frontend

Open auth.html or index.html in your browser to interact with the application.