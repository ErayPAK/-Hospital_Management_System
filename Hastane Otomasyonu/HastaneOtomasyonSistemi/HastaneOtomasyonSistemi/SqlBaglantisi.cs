using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient; // SQL kütüphanesini ekledik.

namespace HastaneOtomasyonSistemi
{
    // ---------------------------------------------------------
    // VERİTABANI BAĞLANTI SINIFI
    // ---------------------------------------------------------
    // Bu sınıf, projenin veritabanına açılan "Kapısı"dır.
    // Her formda tekrar tekrar bağlantı cümlesi yazmamak için bu yapıyı kurduk.
    // Bir değişiklik gerektiğinde (örn: sunucu adı değişirse) sadece burayı değiştirmek yeterli olacaktır.
    class SqlBaglantisi
    {
        // Geriye 'SqlConnection' nesnesi döndüren metodumuz.
        // Public yaptık çünkü projenin her yerinden (diğer formlardan) ulaşabilmemiz lazım.
        public SqlConnection baglanti()
        {
            // 1. BAĞLANTI ADRESİNİN TANIMLANMASI
            // Bu satır veritabanının "Adres Tarifi"dir:
            // Data Source       = Sunucunun adı (Hangi bilgisayar?) -> DESKTOP-ERAY\SQLEXPRESS
            // Initial Catalog   = Veritabanının adı (Hangi klasör?) -> HastaneOtomasyonu
            // Integrated Security= True (Şifre gerekli mi?) -> Windows hesabımla güvenli giriş yap (Bu Uygulama İçin Şifresiz).

            SqlConnection baglan = new SqlConnection("Data Source=DESKTOP-ERAY\\SQLEXPRESS;Initial Catalog=HastaneOtomasyonu;Integrated Security=True");

            // 2. BAĞLANTIYI AÇMA
            // Hattı fiziksel olarak açıyoruz. Bu komut çalışmazsa "Bağlantı Hatası" alırız.
            baglan.Open();

            // 3. BAĞLANTIYI GÖNDERME
            // Açılan bu hattı, bu metodu çağıran yere (Örn: Doktor Paneline) geri gönderiyoruz.
            // Böylece o form, bu açık hat üzerinden verileri çekebilecek.
            return baglan;
        }
    }
}