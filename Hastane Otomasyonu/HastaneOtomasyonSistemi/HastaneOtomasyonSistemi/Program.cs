using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HastaneOtomasyonSistemi
{
    // ---------------------------------------------------------
    // PROJENİN ANA GİRİŞ KAPISI
    // ---------------------------------------------------------
    // Bu sınıf "static"tir çünkü programın hafızasında sadece bir tane olması yeterlidir.
    // Exe dosyasına çift tıkladığında bilgisayarın ilk baktığı yer burasıdır.
    internal static class Program
    {
        // [STAThread] (Single Threaded Apartment):
        // Bu teknik bir koddur. Windows Form uygulamalarının arayüzünün,
        // kopyala-yapıştır özelliklerinin ve sürükle-bırak işlemlerinin düzgün çalışması için
        // uygulamanın "Tek İş Parçacığı" modelinde çalışması gerektiğini işletim sistemine söyler.
        [STAThread]
        static void Main()
        {
            // 1. GÖRSEL AYARLAR:
            // Uygulamanın, çalıştığı Windows sürümünün (Windows 10, 11 vb.) buton ve pencere stillerini kullanmasını sağlar.
            // Bu satır olmazsa butonlar eski Windows 95 tarzı gibi görünür.
            Application.EnableVisualStyles();

            // 2. YAZI AYARLARI:
            // Uygulamadaki yazıların daha modern ve pürüzsüz çizilmesini sağlar.
            // false değeri varsayılan ve önerilen ayardır.
            Application.SetCompatibleTextRenderingDefault(false);

            // 3. PROGRAMI BAŞLATMA:
            // Burası en önemli kısımdır. "Application.Run" komutu sonsuz bir döngü başlatır ve pencereyi ekranda tutar.
            // Parantez içine "new FrmGiris()" yazdığımız için, program açıldığında kullanıcıyı
            // ilk olarak "Giriş Seçim Ekranı" (FrmGiris) karşılar.
            Application.Run(new FrmGiris());
        }
    }
}