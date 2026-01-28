# 🚀 MyPortfolyo - Modern Portfolyo Yönetim Sistemi

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue)
![Status](https://img.shields.io/badge/Status-Active-success)
![License](https://img.shields.io/badge/License-MIT-yellow)


Bu proje, **.NET 8.0** ve **ASP.NET Core MVC** kullanılarak geliştirilmiş, modern ve dinamik bir kişisel portfolyo web uygulamasıdır. Kapsamlı yönetim paneli (Admin Panel) sayesinde sitedeki tüm içerikler (hakkımda, yetenekler, projeler vb.) kod bilgisine ihtiyaç duymadan yönetilebilir.

## 📷 Proje Görselleri

Aşağıdaki bağlantılara tıklayarak projenin ekran görüntülerini inceleyebilirsiniz:

| Bölüm | Ekran Görüntüsü Linki |
| :--- | :--- |
| **Yönetim Paneli (Dashboard)** | [📸 Görseli İncele](https://prnt.sc/x09qb3Dn75g5) |
| **Ana Sayfa (UI)** | [📸 Görseli İncele](https://prnt.sc/JZYoIWKw2wxu) |
| **Mobil Görünüm** | [📸 Görseli İncele](https://prnt.sc/_XK3JZA5Ters) |

---

## ✨ Öne Çıkan Özellikler

### 🛡️ Yönetim Paneli (Admin Panel)
Modern **Glassmorphism** tasarımına sahip, kullanıcı dostu bir yönetim arayüzü sunar.

* **Genel Yönetim**: Site başlığı, logolar, sosyal medya linkleri ve favicon gibi genel ayarları anlık güncelleyin.
* **İçerik Yönetimi**:
    * **Ana Sayfa (Homepage)**: Karşılama metinleri ve özellikleri düzenleyin.
    * **Projeler & Yetenekler**: Portfolyo projelerinizi ve yeteneklerinizi listeyin.
    * **Hizmetler & Referanslar**: Sunduğunuz hizmetleri ve müşteri yorumlarını yönetin.
* **İletişim & Mesajlar**: Site üzerinden gelen iletişim mesajlarını panelden okuyun ve yönetin.
* **Tema Yönetimi**: AnaSayfa için görsel tema ayarları.

### 🌐 Kullanıcı Arayüzü (Public UI)
* **Dinamik İçerik**: Tüm bölümler veritabanından dinamik olarak beslenir.
* **Responsive Tasarım**: Mobil uyumlu ve modern arayüz.

## 🛠️ Teknolojiler

Proje, endüstri standardı teknolojiler ve en iyi uygulama pratikleri (Best Practices) ile geliştirilmiştir:

* **Backend**: 
    * .NET 8.0 (ASP.NET Core MVC)
    * Entity Framework Core 8 (Code First Yaklaşımı)
    * ASP.NET Core Identity (Güvenli Giriş & Yetkilendirme)
    * N-Layer Architecture (Katmanlı Mimari: Web, Data, Entities)
* **Frontend**: 
    * Razor Views.
    * HTML5, CSS3 (Glassmorphism efektleri).
    * Bootstrap.
* **Veritabanı**: 
    * MSSQL (Microsoft SQL Server).

## 🚀 Kurulum ve Çalıştırma

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları izleyin:

1.  **Projeyi Klonlayın**:
    ```bash
    git clone [https://github.com/brhnshn/MyPortfolio.git](https://github.com/brhnshn/MyPortfolio.git)
    cd MyPortfolio
    ```

2.  **Veritabanı Bağlantısını Ayarlayın**:
    `appsettings.json` dosyasındaki `ConnectionStrings` bölümünü kendi SQL Server bilgilerinize göre güncelleyin.

3.  **Veritabanını Oluşturun (Migration)**:
    Terminal veya Package Manager Console üzerinden migration'ları uygulayın:
    ```bash
    dotnet ef database update
    ```

4.  **Projeyi Başlatın**:
    ```bash
    dotnet run
    ```
    Tarayıcınızda `https://localhost:5001` (veya belirtilen port) adresine giderek uygulamayı görüntüleyebilirsiniz.

## 📂 Proje Yapısı

* `MyPortfolio.Entities`: Veritabanı tablolarına karşılık gelen varlık sınıfları.
* `MyPortfolio.Data`: Veritabanı erişim katmanı (Context, Repository'ler).
* `MyPortfolio`: Ana web uygulaması (Controller'lar, View'lar).

---

## 📬 İletişim

Geri bildirim, öneri veya iş birliği için:

* **E‑posta:** [sahinburhan501@gmail.com](mailto:sahinburhan501@gmail.com)
* **GitHub:** [https://github.com/brhnshn](https://github.com/brhnshn)
* **Linkedin:** [https://www.linkedin.com/in/burhan-sahin/](https://www.linkedin.com/in/burhan-sahin/)

---

## 📄 Lisans

Bu proje **MIT License** ile lisanslanmıştır.
