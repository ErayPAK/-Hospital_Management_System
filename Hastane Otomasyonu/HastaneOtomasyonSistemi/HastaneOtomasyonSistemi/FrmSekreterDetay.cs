using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace HastaneOtomasyonSistemi
{
    public partial class FrmSekreterDetay : Form
    {
        // ---------------------------------------------------------
        // GLOBAL DEĞİŞKENLER
        // ---------------------------------------------------------
        public string TCnumara; // Giriş formundan taşınan Sekreter TC'si
        SqlBaglantisi bgl = new SqlBaglantisi();

        public FrmSekreterDetay()
        {
            InitializeComponent();
        }


        void YenidIdGetir()
        {
            // Veritabanına bağlanıp en son eklenen RandevuID'yi soracağız
            // MAX(Randevuid) komutu en büyük sayıyı verir.

            SqlCommand komut = new SqlCommand("Select MAX(Randevuid) From Tbl_Randevular", bgl.baglanti());

            // ExecuteScalar, tek bir değer (hücre) döndürür.
            object sonuc = komut.ExecuteScalar();

            // Eğer veritabanı boşsa (hiç randevu yoksa) sonuç null gelir.
            // DBNull.Value kontrolü yapıyoruz.
            if (sonuc != DBNull.Value && sonuc != null)
            {
                // En büyük sayıyı al, 1 ekle ve kutuya yaz.
                int enBuyukId = Convert.ToInt32(sonuc);
                Txtid.Text = (enBuyukId + 1).ToString();
            }
            else
            {
                // Eğer tablo boşsa ilk kayıt 1 numara olacaktır.
                Txtid.Text = "1";
            }

            bgl.baglanti().Close();
        }







        void HastalariListele(string kelime)
        {
            // 1. Tabloyu temizle veya yeni oluştur
            DataTable dt = new DataTable();

            // 2. "sp_HastaArama" prosedürünü çağırıyoruz
            SqlCommand komut = new SqlCommand("sp_HastaArama", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure;

            // 3. Parametreyi gönderiyoruz sp'deki @aranacakKelime
            komut.Parameters.AddWithValue("@AranacakKelime", kelime);

            // 4. Verileri çekip tabloya dolduruyoruz
            SqlDataAdapter da = new SqlDataAdapter(komut);
            da.Fill(dt);

           
            dataGridView3.DataSource = dt;

        }



        // ---------------------------------------------------------
        // LİSTEDEN SEÇİM
        // ---------------------------------------------------------
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Sekreter listeden bir randevuya tıkladığında, o randevunun ID'sini alıp
            // gizli bir TextBox'a yazıyoruz.
            // İptal butonuna basınca bu ID'yi kullanacağız.
            int secilen = dataGridView1.SelectedCells[0].RowIndex;
            TxtRandevuId.Text = dataGridView1.Rows[secilen].Cells[0].Value.ToString();
        }

        // ---------------------------------------------------------
        // BOŞ SAATLERİ HESAPLAMA
        // ---------------------------------------------------------
        void SaatleriListele()
        {
            // KORUMA: Eğer Doktor veya Branş seçilmemişse hesaplama yapma, çık.
            if (string.IsNullOrEmpty(CmbBrans.Text) || string.IsNullOrEmpty(CmbDoktor.Text))
            {
                return;
            }

            CmbSaat.Items.Clear(); // Listeyi temizle ki eski saatler kalmasın.

            // DOLU SAATLERİ BELİRLEME
            // Veritabanına gidip "Bu doktorun, bu tarihte hangi saatleri dolu?" diye soruyoruz.
            SqlCommand komut = new SqlCommand("Select RandevuSaat From Tbl_Randevular where RandevuBrans=@p1 AND RandevuDoktor=@p2 AND RandevuTarih=@p3", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", CmbBrans.Text);
            komut.Parameters.AddWithValue("@p2", CmbDoktor.Text);
            komut.Parameters.AddWithValue("@p3", DtpTarih.Value.ToString("yyyy-MM-dd"));

            SqlDataReader dr = komut.ExecuteReader();

            // Veritabanından gelen dolu saatleri geçici bir listeye (List<string>) atıyoruz.
            // Bu bizim "Yasaklı Saatler" listemiz olacak.
            List<string> doluSaatler = new List<string>();
            while (dr.Read())
            {
                doluSaatler.Add(dr[0].ToString());
            }
            bgl.baglanti().Close();



            // TÜM MESAİ DİLİMLERİNİ OLUŞTURMA VE FİLTRELEME
            // Mesai 09:00'da başlar, 16:00'da biter.
            DateTime baslangic = DateTime.Parse("09:00");
            DateTime bitis = DateTime.Parse("16:00");

            while (baslangic <= bitis)
            {
                string saatDegeri = baslangic.ToString("HH:mm");

                // Eğer oluşturduğumuz saat, "doluSaatler" listesinde YOKSA;
                // demek ki o saat boştur. ComboBox'a ekleyebiliriz.
                if (!doluSaatler.Contains(saatDegeri))
                {
                    CmbSaat.Items.Add(saatDegeri);
                }

                baslangic = baslangic.AddMinutes(15); // 15'er dakika artır.
            }
        }

        // ---------------------------------------------------------
        // FORM YÜKLENİRKEN
        // ---------------------------------------------------------
        private void FrmSekreterDetay_Load(object sender, EventArgs e)
        {
            LblTC.Text = TCnumara; // Sekreterin TC'sini yaz.

            // 1. SEKRETER BİLGİLERİ
            SqlCommand komut1 = new SqlCommand("Select SekreterAdSoyad From Tbl_Sekreterler where SekreterTC=@p1", bgl.baglanti());
            komut1.Parameters.AddWithValue("@p1", LblTC.Text);
            SqlDataReader dr1 = komut1.ExecuteReader();
            while (dr1.Read())
            {
                LblAdSoyad.Text = dr1[0].ToString();
            }
            bgl.baglanti().Close();

            // 2. BRANŞLARI TABLOYA AKTARMA
            DataTable dt1 = new DataTable();
            SqlDataAdapter da1 = new SqlDataAdapter("Select * From Tbl_Branslar", bgl.baglanti());
            da1.Fill(dt1);
            dataGridView1.DataSource = dt1;

            // 3. DOKTORLARI TABLOYA AKTARMA 
            // Burada SQL tarafında (Ad + Soyad) birleştirmesi yaparak tek sütunda gösteriyoruz.
            DataTable dt2 = new DataTable();
            SqlDataAdapter da2 = new SqlDataAdapter("Select (DoktorAd + ' ' + DoktorSoyad) as 'Doktorlar', DoktorBrans From Tbl_Doktorlar", bgl.baglanti());
            da2.Fill(dt2);
            dataGridView2.DataSource = dt2;

            // 4. BRANŞLARI COMBOBOX'A AKTARMA
            SqlCommand komut2 = new SqlCommand("Select BransAd From Tbl_Branslar", bgl.baglanti());
            SqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                CmbBrans.Items.Add(dr2[0]);
            }
            bgl.baglanti().Close();

            // Geçmişe randevu verilmesini engelle.
            DtpTarih.MinDate = DateTime.Now;

            HastalariListele(""); // Boş gönderiyoruz ki hepsi listelensin

            YenidIdGetir(); // Form açılınca sıradaki numarayı hesapla ve yaz.
        }

        // ---------------------------------------------------------
        // KAYDET BUTONU
        // ---------------------------------------------------------
        private void BtnKaydet_Click(object sender, EventArgs e)
        {
            // Normal INSERT sorgusu yerine, "sp_RandevuAl_Guvenli" isimli prosedürü kullanıyoruz.
            // Bu prosedür sadece kayıt yapmaz, aynı zamanda bize bir mesaj döndürür.

            SqlCommand komutkaydet = new SqlCommand("sp_RandevuAl_Guvenli", bgl.baglanti());
            komutkaydet.CommandType = CommandType.StoredProcedure;

            // 1. GİRDİ PARAMETRELERİ
            komutkaydet.Parameters.AddWithValue("@Tarih", DtpTarih.Value.ToString("yyyy-MM-dd"));
            komutkaydet.Parameters.AddWithValue("@Saat", CmbSaat.Text);
            komutkaydet.Parameters.AddWithValue("@Brans", CmbBrans.Text);
            komutkaydet.Parameters.AddWithValue("@Doktor", CmbDoktor.Text);
            komutkaydet.Parameters.AddWithValue("@HastaTC", MskTC.Text);
            komutkaydet.Parameters.AddWithValue("@Sikayet", "Sekreter Girişi");

            // 2. ÇIKTI PARAMETRESİ 
            // SQL'den C#'a veri taşıyoruz. SQL işlemi yapacak ve sonucu bu parametreye yazacak.
            SqlParameter outParam = new SqlParameter();
            outParam.ParameterName = "@SonucMesaj";     // SQL'deki değişken adı
            outParam.SqlDbType = SqlDbType.VarChar;     // Türü
            outParam.Size = 100;                        // Boyutu
            outParam.Direction = ParameterDirection.Output; // Yönü: ÇIKIŞ (SQL -> C#)
            komutkaydet.Parameters.Add(outParam);



            // 3. ÇALIŞTIR
            komutkaydet.ExecuteNonQuery();

            // 4. CEVABI OKU VE GÖSTER
            // SQL'in output parametresine yazdığı mesajı alıyoruz.
            string gelenMesaj = komutkaydet.Parameters["@SonucMesaj"].Value.ToString();

            if (gelenMesaj.Contains("HATA"))
            {
                // SQL "HATA" kelimesini içeren bir mesaj döndürdüyse (Örn: "Dolu Randevu!")
                MessageBox.Show(gelenMesaj, "Randevu Oluşturulamadı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Başarılıysa
                MessageBox.Show(gelenMesaj, "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            YenidIdGetir();

            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // BRANŞ SEÇİLİNCE
        // ---------------------------------------------------------
        private void CmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Branş seçilince, o branşa ait doktorları getiriyoruz.
            CmbDoktor.Items.Clear();
            CmbSaat.Items.Clear(); // Branş değişince saatleri de sıfırla.

            SqlCommand komut = new SqlCommand("Select DoktorAd, DoktorSoyad From Tbl_Doktorlar where DoktorBrans=@p1", bgl.baglanti());
            komut.Parameters.AddWithValue("@p1", CmbBrans.Text);
            SqlDataReader dr = komut.ExecuteReader();
            while (dr.Read())
            {
                CmbDoktor.Items.Add(dr[0] + " " + dr[1]);
            }
            bgl.baglanti().Close();
        }

        // ---------------------------------------------------------
        // DİĞER BUTONLAR
        // ---------------------------------------------------------
        private void BtnDuyuruOlustur_Click(object sender, EventArgs e)
        {
            // Duyuru Paneli için basit insert işlemi
            SqlCommand komut = new SqlCommand("insert into Tbl_Duyurular (Duyuru) values (@d1)", bgl.baglanti());
            komut.Parameters.AddWithValue("@d1", RchDuyuru.Text);
            komut.ExecuteNonQuery();
            bgl.baglanti().Close();
            MessageBox.Show("Duyuru Oluşturuldu");
        }

        private void BtnDoktorPanel_Click(object sender, EventArgs e)
        {
            FrmDoktorPaneli fr = new FrmDoktorPaneli();
            fr.Show();
        }

        private void BtnBransPanel_Click(object sender, EventArgs e)
        {
            FrmBransPaneli fr = new FrmBransPaneli();
            fr.Show();
        }

        private void BtnRandevuListe_Click(object sender, EventArgs e)
        {
            FrmRandevuListesi fr = new FrmRandevuListesi();
            fr.Show();
        }

        // ---------------------------------------------------------
        // TARİH VEYA DOKTOR DEĞİŞİNCE SAATLERİ GÜNCELLE
        // ---------------------------------------------------------
        private void DtpTarih_ValueChanged(object sender, EventArgs e)
        {
            SaatleriListele();
        }

        private void CmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaatleriListele();
        }

        // ---------------------------------------------------------
        // İPTAL BUTONU 
        // ---------------------------------------------------------
        private void BtnIptal_Click(object sender, EventArgs e)
        {
            if (TxtRandevuId.Text == "")
            {
                MessageBox.Show("Lütfen iptal edilecek randevuyu listeden seçiniz.");
                return;
            }

            SqlCommand komut = new SqlCommand("sp_RandevuIptalEt", bgl.baglanti());
            komut.CommandType = CommandType.StoredProcedure;

            // Parametre: Silinecek Randevu ID
            komut.Parameters.AddWithValue("@RandevuID", TxtRandevuId.Text);

            // GERİ DÖNÜŞ DEĞERİ YAKALAMA
            // Kaydet butonunda "Output" kullanmıştık (Metin dönüyordu).
            // Burada "Return Value" kullanıyoruz (Sayı dönüyor: 1 veya 0).
            SqlParameter returnDeger = new SqlParameter();
            returnDeger.Direction = ParameterDirection.ReturnValue;
            komut.Parameters.Add(returnDeger);

            komut.ExecuteNonQuery();

            // SQL'den dönen sayısal sonucu al.
            int sonuc = (int)returnDeger.Value;

            if (sonuc == 1)
            {
                // SQL "1" döndürdüyse işlem başarılıdır.
                MessageBox.Show("Randevu başarıyla iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // SQL "0" veya başka bir kod döndürdüyse işlem başarısızdır (Örn: Geçmiş tarih).
                MessageBox.Show("Geçmiş tarihli randevular iptal edilemez!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bgl.baglanti().Close();
        }

        private void TxtHastaAra_TextChanged(object sender, EventArgs e)
        {
            // Kullanıcı her harfe bastığında listeyi o kelimeye göre güncelle
            HastalariListele(TxtHastaAra.Text);
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0) return; // Başlığa tıklamayı engelle

            // Seçilen satırı al
            int secilen = dataGridView3.SelectedCells[0].RowIndex;

            // SP'niz "Select * From" dediği için tüm sütunlar geliyor.
            // Veritabanındaki sütun sırasına göre TC'nin indeksini bulmalıyız.
            // Genelde: id(0), Ad(1), Soyad(2), TC(3)... şeklindedir.
            // Garanti olsun diye sütun isminden de çekebilirsiniz:

            string secilenTC = dataGridView3.Rows[secilen].Cells["HastaTC"].Value.ToString();

            // Randevu panelindeki TC kutusuna aktar
            MskTC.Text = secilenTC;
        }
    }
}