"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/portfolioHub")
    .withAutomaticReconnect()
    .build();

// Sunucudan (server) Component guncellenme sinyali geldiginde
connection.on("UpdateComponent", function (componentName) {
    if (!componentName) return;

    // Sitedeki "component-Isim" ID'li tum elementleri bul (Sayfada birden fazla yerde kullanilmis olabilir)
    const elements = document.querySelectorAll("#component-" + componentName);

    if (elements && elements.length > 0) {
        // Fetch ile o componentin yeni html render halini al
        fetch("/Home/RenderComponent?componentName=" + componentName)
            .then(response => {
                if (response.ok) return response.text();
                throw new Error('Component render edilemedi.');
            })
            .then(html => {
                // Bulunan tum div'lerin icerigini aninda guncelle (sayfa yenilemeden)
                elements.forEach(el => {
                    el.innerHTML = html;
                });

                // Eger AOS animasyonlari sayfasinda (varsa) yeniden hesaplansin
                if (typeof AOS !== 'undefined') {
                    setTimeout(() => AOS.refreshHard(), 100);
                }

                // Tema dosyalari tarafindan eklenen diger event dinleyicilerini tetikleyebilmek icin:
                window.dispatchEvent(new Event('resize'));
            })
            .catch(error => { }); // Hata yutulur, konsola basilmaz
    }
});

// Baglantiyi baslat
connection.start().catch(function (err) {
    // Baglanti hatalari yutulur
});
