$(document).ready(function () {

    // 1. AOS (Animasyon Kütüphanesi) Başlatma
    if (typeof AOS !== 'undefined') {
        AOS.init();
    }

    // 2. Navbar Scroll Efekti
    window.addEventListener('scroll', function () {
        if (window.scrollY > 50) {
            document.getElementById('mainNav').classList.add('shadow-sm');
            document.getElementById('mainNav').style.paddingTop = "10px";
            document.getElementById('mainNav').style.paddingBottom = "10px";
        } else {
            document.getElementById('mainNav').classList.remove('shadow-sm');
            document.getElementById('mainNav').style.paddingTop = "15px";
            document.getElementById('mainNav').style.paddingBottom = "15px";
        }
    });

    // 3. Mobil Menü Kapatma
    $('.nav-link').on('click', function () {
        $('.navbar-collapse').collapse('hide');
    });

    // 4. Yukarı Çık Butonu
    var backToTop = document.querySelector('.back-to-top');
    if (backToTop) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 100) {
                backToTop.classList.add('active');
            } else {
                backToTop.classList.remove('active');
            }
        });
    }

    // 5. Hero Bölümü 3D Fotoğraf Efekti (FeatureList'ten taşındı)
    const container = document.getElementById('tiltContainer');
    const img = document.getElementById('tiltImage');

    if (container && img) {
        // Mouse hareket ettiğinde
        container.addEventListener('mousemove', (e) => {
            const rect = container.getBoundingClientRect();
            // Mouse'un container içindeki konumu
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            // Konumu merkeze göre hesapla
            const xPct = (x / rect.width) - 0.5;
            const yPct = (y / rect.height) - 0.5;
            // Dönüş derecesini ayarla
            const rotateY = xPct * 15;
            const rotateX = -yPct * 15;

            img.style.transform = `rotateY(${rotateY}deg) rotateX(${rotateX}deg) scale(1.05)`;
        });

        // Mouse dışarı çıkınca eski haline dönsün
        container.addEventListener('mouseleave', () => {
            img.style.transform = `rotateY(0deg) rotateX(0deg) scale(1)`;
        });
    }
});