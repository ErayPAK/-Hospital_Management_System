



/**********************CREATE TABLE metodu kullanýlarak veritabanýna yeni tablolar eklendim.****************************/
 



-- 1. Branþlar Tablosu
--Branþlarý tutacak olan tablo.
CREATE TABLE Tbl_Branslar 
(
    Bransid tinyint PRIMARY KEY IDENTITY(1,1),
    BransAd varchar(30)
);

-- 2. Doktorlar Tablosu
--Doktorlarý ve bilgilerini tutacak olan tablo.
CREATE TABLE Tbl_Doktorlar 
(
    Doktorid smallint PRIMARY KEY IDENTITY(1,1),    --Bu sütun bu tablonun kimliðidir. ID numarasý 1 den baþlar ve 1'er 1'er artar.
    DoktorAd varchar(20),
    DoktorSoyad varchar(20),
    DoktorBrans varchar(30),
    DoktorTC char(11),
    DoktorSifre varchar(10)
);

-- 3. Hastalar Tablosu
--Hastalarýný ve bilgilerini tutacak olan tablo.
CREATE TABLE Tbl_Hastalar 
(
    Hastaid smallint PRIMARY KEY IDENTITY(1,1),     --Bu sütun bu tablonun kimliðidir. ID numarasý 1 den baþlar ve 1'er 1'er artar.
    HastaAd varchar(20),
    HastaSoyad varchar(20),
    HastaTC char(11),
    HastaTelefon varchar(15),
    HastaSifre varchar(10),
    HastaCinsiyet varchar(5)
);

-- 4. Sekreterler Tablosu
--Sekreterleri ve bilgilerini tutacak olan tablo.
CREATE TABLE Tbl_Sekreterler 
(
    Sekreterid tinyint PRIMARY KEY IDENTITY(1,1),   --Bu sütun bu tablonun kimliðidir. ID numarasý 1 den baþlar ve 1'er 1'er artar.
    SekreterAdSoyad varchar(30),
    SekreterTC char(11),
    SekreterSifre varchar(10)
);

-- 5. Randevular Tablosu (Ýliþkilerin merkezi)
--Randevularý ve randevular hakkýndakþ bilgileri tutan tablo.
CREATE TABLE Tbl_Randevular 
(
    Randevuid int PRIMARY KEY IDENTITY(1,1),    --Bu sütun bu tablonun kimliðidir. ID numarasý 1 den baþlar ve 1'er 1'er artar.
    RandevuTarih date,
    RandevuSaat varchar(5),
    RandevuBrans varchar(30),
    RandevuDoktor varchar(20),
    RandevuDurum bit DEFAULT 0, 
    HastaTC char(11),
    HastaSikayet varchar(250),
    RandevuOnay bit DEFAULT 0 --Hastaya "Randevuya gelecek misiniz?" diye soru sorduktan sonra cevabý kaydetmek için aldýðýmýz sütun.
);

-- 6. Duyurular Tablosu
--Admin/sekreter duyuru oluþturursa oluþturulan duyurunun saklanacaðý tabllo.
CREATE TABLE Tbl_Duyurular 
(
    Duyuruid int PRIMARY KEY IDENTITY(1,1),     --Bu sütun bu tablonun kimliðidir. ID numarasý 1 den baþlar ve 1'er 1'er artar.
    Duyuru varchar(200)
);

-- 7. Testler/Laboratuvar Tablosu
--Hastanýn kan tahlili ve röntgen görünntülerinin saklandýðý tablo.
CREATE TABLE Tbl_Testler 
(
    Testid int PRIMARY KEY IDENTITY(1,1),       --Bu sütun bu tablonun kimliðidir. ID numarasý 1 den baþlar ve 1'er 1'er artar.
    HastaTC char(11),
    TestAd varchar(50), --TestAd kan tahlili ve röntgen olabilir.
    TestSonuc varchar(100),
    DoktorOnay bit DEFAULT 0
);


-- 8.Yönetici Tablosu
CREATE TABLE Tbl_Yonetici 
(
    Yoneticiid int PRIMARY KEY IDENTITY(1,1),
    KullaniciAd varchar(20),
    Sifre varchar(20)
);

-- Ýlk yöneticiyi ekledim INSERT INTO ile (Kullanýcý Adý: admin, Þifre: admin1234)
INSERT INTO Tbl_Yonetici (KullaniciAd, Sifre) VALUES ('admin', 'admin1234');