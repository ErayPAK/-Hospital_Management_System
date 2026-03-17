
                                                                                                    --TARÝH:02/12/2025
/**********************STORED PROCEDURE' LER *************************/

--Bu stored procedure' ler C# tarafýnda kod yazarken gerçekten bana bayaðý bir kolaylýk saðladý.
--Ýlk 5 prosedür çok fazla iþleve sahip olmayan ama bana kolaylýk saðlayan prosedürlerdir.
--Diðer 5 prosedür ise daha yüksek iþleve sahip sonradan eklenmiþ prosdürlerdir.

USE HastaneOtomasyonu;
                                                                                            
GO
-- STORED PROCEDURE 1: Hasta Giriþ Kontrolü
/********************************************************************************
* PROSEDÜR ADI: sp_HastaGiris
* AMAÇ: Hasta giriþ panelinde kimlk doðrulama iþlemini yapmak.
* MANTIK: Kullanýcýnýn girdiði TC ve Þifre bilgisini parametre olarak alýr.
* Veritabanýndaki kayýtlarla eþleþip eþleþmediðini kontrol eder.
* Eðer bilgiler doðruysa, hastanýn profil bilgilerini programa gönderir.
********************************************************************************/

CREATE PROCEDURE sp_HastaGiris
    -- GÝRÝÞ PARAMETRELERÝ (C# formundan gelen veriler)
    @TC char(11),       -- Kullanýcýnýn girdiði 11 haneli TC Kimlik Numarasý
    @Sifre varchar(10)  -- Kullanýcýnýn girdiði þifre
AS
BEGIN
    -- DOÐRULAMA VE VERÝ ÇEKME SORGUSU
    -- 'SELECT *' kullanmamýzýn sebebi: 
    -- Giriþ iþlemi baþarýlý olursa, açýlacak olan Hasta Detay formunda 
    -- Hoþgeldiniz diyebilmek için hastanýn tüm bilgilerine ihtiyacýmýz var.
    
    SELECT * FROM Tbl_Hastalar 
    WHERE HastaTC = @TC AND HastaSifre = @Sifre

    -- ÇALIÞMA MANTIÐI:
    -- 1. Eðer TC ve Þifre doðruysa: Veritabaný o hastaya ait satýrý döndürür.
    --    C# tarafýnda (if dr.Read()) bloðu çalýþýr ve giriþ yapýlýr.
    -- 2. Eðer bilgiler yanlýþsa: Veritabaný boþ deðer döndürür.
    --    C# tarafýnda (else) bloðu çalýþýr ve "Hatalý Giriþ" mesajý verilir.
END;
GO







-- STORED PROCEDURE 2: Doktor Giriþ Kontrolü
/********************************************************************************
* PROSEDÜR ADI: sp_DoktorGiris
* AMAÇ: Doktor giriþ panelinde güvenlik kontrolü yapmak.
* MANTIK: Formdan gönderilen TC ve Þifre bilgisini alýr.
* 'Tbl_Doktorlar' tablosunda bu bilgilere sahip bir doktor var mý diye bakar.
* Eþleþme saðlanýrsa doktorun bilgilerini döndürür ve sisteme giriþ izni verir.
********************************************************************************/

CREATE PROCEDURE sp_DoktorGiris
    -- GÝRÝÞ PARAMETRELERÝ (C#'tan geliyr)
    @TC char(11),       -- Doktorun giriþ yapmak için kullandýðý TC Kimlik Numarasý
    @Sifre varchar(10)  -- Doktorun sisteme giriþ þifresi
AS
BEGIN
   
    SELECT * FROM Tbl_Doktorlar 
    WHERE DoktorTC = @TC AND DoktorSifre = @Sifre       --Eþleþme olursa Tbl_Doktorlar tablosundan o satýrdaki bilgileri çekiyoruz.

    -- ÇALIÞMA MANTIÐI:
    -- 1. Eþleþme VARSA: Veritabaný o doktora ait satýrý getirir.
    -- 2. Eþleþme YOKSA: Veritabaný boþ sonuç döndürür.
END;
GO






-- STORED PROCEDURE 3: Sekreter Giriþ Kontrlü
/********************************************************************************
* PROSEDÜR ADI: sp_SekreterGiris
* AMAÇ: Sekreter giriþ panelinde kimlk doðrulama iþlemini yapmak.
*  MANTIK: 
*Kullanýcýdan gelen TC ve Þifre bilgisini alýr.
* 'Tbl_Sekreterler' tablosunda bu kaydýn olup olmadýðýný kontrol eder.
* Eðer bilgiler doruysa, sekreterin yetkilerini ve bilgilerini programa döndürür.
********************************************************************************/

CREATE PROCEDURE sp_SekreterGiris
    -- GÝRÝÞ PARAMETRELERÝ
    @TC char(11),       -- Sekreterin giriþ yapmak için kullandýðý TC Kimlik Numarasý
    @Sifre varchar(10)  -- Sekreterin sisteme giriþ þifresi
AS
BEGIN

    SELECT * FROM Tbl_Sekreterler 
    WHERE SekreterTC = @TC AND SekreterSifre = @Sifre

    -- ÇALIÞMA MANTIÐI:
    -- 1. Eþleþme VARSA: Veritabaný sekretere ait satýrý döndürür.
    -- 2. Eþleþme YOKSA: Veritabaný boþ deðer döndürür.
END;
GO






-- STORED PROCEDURE 4: Randevu Alma
/********************************************************************************
* PROSEDÜR ADI: sp_RandevuAl
* AMAÇ: Sisteme yeni bir randevu kaydý eklemek.
* MANTIK: Kullanýcýdan yani hasta ve sekreterdenn gelen randevu bilgilerini parametre olarak alýr.
* Bu bilgileri 'Tbl_Randevular' tablosuna yeni bir satýr olarak ekler (INSERT INTO komutu ile).
* Randevu durumu otomatik olarak '1' (Dolu) olarak iþaretlenir.
(Burayý ilerideki Prosedürde deðiþtirdim.)
********************************************************************************/

CREATE PROCEDURE sp_RandevuAl
    -- GÝRÝÞ PARAMETRELERÝ (Formdan gelen veriler)
    @Tarih date,           -- Randevunun tarihi
    @Saat varchar(5),      -- Randevunun saati (Örn: 10:30)
    @Brans varchar(30),    -- Poliklinik/Branþ adý
    @Doktor varchar(20),   -- Doktorun adý
    @HastaTC char(11),     -- Randevuyu alan hastanýn TC'si
    @Sikayet varchar(250)  -- Hastanýn þikayeti
AS
BEGIN
    -- VERÝ EKLEME ÝÞLEMÝ (INSERT INTO)
    -- Parametre olarak gelen deðerleri, veritabanýndaki ilgili sütunlara yerleþtirir.
    
    INSERT INTO Tbl_Randevular (RandevuTarih, RandevuSaat, RandevuBrans, RandevuDoktor, HastaTC, HastaSikayet, RandevuDurum)
    VALUES (@Tarih, @Saat, @Brans, @Doktor, @HastaTC, @Sikayet, 1)
    
    -- DÝKKAT EDÝLMESÝ GEREKEN NOKTA:
    -- 'RandevuDurum' parametre olarak alýnmaz, el ile '1' olarak gönderilir.
    -- 1: Dolu/Alýnmýþ randevu anlamýna gelir. 
    -- 0: Boþ randevu anlamýna gelir (Bu prosedür çalýþtýðýnda randevu alýndýðý için direkt 1 yapýyoruz).
    --(Deðiþen kýsým tam olarak burasý. Zaman kýsýtlý olduðu için bu prosedür ile ilgili bütün bilgileri kaldýrmakla uðraþmadým.)
END;
GO





-- STORED PROCEDURE 5: Duyuru Ekleme 
/********************************************************************************
* PROSEDÜR ADI: sp_DuyuruEkle
* AMAÇ: Hastane otomasyon sistemine genel bir duyuru/haber eklemek.
* MANTIK: Sekreter veya Yönetici panelinden girilen duyuru metnini parametre olarak alýr.
* Bu metni 'Tbl_Duyurular' tablosuna yeni bir satýr olarak ekler.
* Eklenen bu duyuru, doktorlarýn ve diðer kullanýcýlarýn ekranýndaki "Duyurular"
* listesinde otomatik olarak görünür hale gelir.
********************************************************************************/

CREATE PROCEDURE sp_DuyuruEkle
    -- GÝRÝÞ PARAMETRESÝ
    @DuyuruIcerik varchar(200)  -- Kullanýcýnýn yazdýðý duyuru metni (Maksimum 200 karakter olarakm seçtim.)
AS
BEGIN
    -- VERÝ EKLEME ÝÞLEMÝ (INSERT INTO )
    -- Parametre olarak gelen metni, veritabanýndaki duyurular tablosuna kaydeder.
    
    INSERT INTO Tbl_Duyurular (Duyuru) 
    VALUES (@DuyuruIcerik)
    
    -- Bu iþlemden sonra C# tarafýnda "Duyuru Oluþturuldu" mesaýj gelir.
END;
GO




                                                                                                    --TARÝH:21/12/2025

-- STORED PROCEDURE 6: Geliþmiþ Randevu Alma : Önceki Randevu Almanýn Geliþmiþ Versiyonu
/********************************************************************************
* PROSEDÜR ADI: sp_RandevuAl_Guvenli
* AMAÇ: Randevu çakýþmalarýný önleyerek güvenli kayýt yapmak.
* MANTIK: 
* 1. Doktorun o saatte baþka hastasý var mý?
* 2. Hastanýn o saatte baþka randevusu var mý?
* 3. Her þey uygunsa kayýt yap, deðilse iþlemi geri al (Rollback)[Burasý önemli bir kýsýn.].
********************************************************************************/

CREATE PROCEDURE sp_RandevuAl_Guvenli
    -- GÝRÝÞ PARAMETRELERÝ (C# tarafýndan gönderilen veriler)
    @Tarih date,            -- Randevunun tarihi
    @Saat varchar(5),       -- Randevunun saati
    @Brans varchar(30),     -- Branþ (Dahiliye vb.)
    @Doktor varchar(50),    -- Doktorun Adý Soyadý
    @HastaTC char(11),      -- Randevuyu alan hastanýn TC'si
    @Sikayet varchar(250),  -- Hastanýn þikayeti

    -- ÇIKIÞ PARAMETRESÝ (SQL'den C# tarafýna geri gönderilecek cevap)
    @SonucMesaj varchar(100) OUTPUT 
AS
BEGIN
    -- TRANSACTION BAÞLATILIYOR 
    -- Transaction, iþlemlerin "Ya hep ya hiç" mantýðýyla çalýþmasýný saðlar. Bu kýsým çok önemli.
    -- Eðer arada herhangi bir iþlemde bir tane dahi hata olursa yapýlan tüm iþlemler geri alýnýr.
    BEGIN TRANSACTION

    -- HATA YAKALAMA BLOÐU (TRY-CATCH)
    BEGIN TRY
        
        -- ---------------------------------------------------------
        -- 1. KONTROL: DOKTOR MÜSAÝT MÝ?
        -- ---------------------------------------------------------
        -- Veritabanýnda ayný doktorun, ayný tarih ve saatte, aktif (Durum=1) bir randevusu var mý?
        IF EXISTS (SELECT 1 FROM Tbl_Randevular WHERE RandevuDoktor = @Doktor AND RandevuTarih = @Tarih AND RandevuSaat = @Saat AND RandevuDurum = 1)
        BEGIN
            -- Eðer kayýt varsa, doktora o saatte randevu verilemez.
            SET @SonucMesaj = 'HATA: Seçilen saatte doktorun baþka bir randevusu mevcut.';
            
            -- Ýþlemi iptal et ve geri al (Veritabanýna hiçbir þey kaydetme) TRANSACTION tam olarak bu iþte.
            ROLLBACK TRANSACTION; 
            
            -- Prosedürden çýk, kodu daha fazla çalýþtýrma
            RETURN;
        END

        -- ---------------------------------------------------------
        -- 2. KONTROL: HASTA MÜSAÝT MÝ?
        -- ---------------------------------------------------------
        -- Ayný hastanýn, ayný tarih ve saatte baþka bir doktora randevusu var mý?
        IF EXISTS (SELECT 1 FROM Tbl_Randevular WHERE HastaTC = @HastaTC AND RandevuTarih = @Tarih AND RandevuSaat = @Saat AND RandevuDurum = 1)
        BEGIN
            -- Eðer varsa, hasta ayný anda iki yerde olamaz.
            SET @SonucMesaj = 'HATA: Ayný saatte baþka bir randevunuz bulunmaktadýr.';
            
            -- Ýþlemi iptal et ve geri al
            ROLLBACK TRANSACTION;
            
            -- Prosedürden çýk
            RETURN;
        END

        -- ---------------------------------------------------------
        -- 3. KAYIT ÝÞLEMÝ
        -- ---------------------------------------------------------
        -- Yukarýdaki kontrollerden baþarýyla geçtiyse randevuyu tabloya ekle.
        INSERT INTO Tbl_Randevular (RandevuTarih, RandevuSaat, RandevuBrans, RandevuDoktor, HastaTC, HastaSikayet, RandevuDurum)
        VALUES (@Tarih, @Saat, @Brans, @Doktor, @HastaTC, @Sikayet, 1); -- 1: Aktif Randevu demektir.

        -- Ýþlem baþarýlý olduðu için C#'a baþarý mesajý gönder
        SET @SonucMesaj = 'BAÞARILI: Randevunuz oluþturuldu.';
        
        -- Ýþlemi veritabanýna kalýcý olarak iþle. Yani kaydet.
        COMMIT TRANSACTION; 

    END TRY
    BEGIN CATCH
        -- ---------------------------------------------------------
        -- HATA DURUMU: EKSTRADAN SQL HATASI OLURSA
        -- ---------------------------------------------------------
        -- Eðer yukarýdaki iþlemlerde SQL kaynaklý sistemsel bir hata olursa buraya düþer.
        
        -- Sistemin verdiði hata mesajýný yakala ve deðiþkene ata
        SET @SonucMesaj = 'SÝSTEM HATASI: ' + ERROR_MESSAGE();
        
        -- Olasý yarým kalmýþ iþlemleri temizle. Yani hiçbir iþlem yapma.
        ROLLBACK TRANSACTION;
    END CATCH
END;
GO







-- STORED PROCEDURE 7: Admin Paneline Doktot, Hasta, Randevu Sayýlarýný Ve En Çok Randevu Alýnmýþ Branþý Getirme
/********************************************************************************
* PROSEDÜR ADI: sp_AdminPanelIstatistikleri
* AMAÇ: Yönetici paneli için özet durum raporu oluþturmak.
* MANTIK: 
* Bu prosedür çalýþtýðýnda veritabanýndaki 3 ana tabloyu tarar, sayýlarý hesaplar,
* istatistiksel analiz yaparak en popüler branþý bulur ve tüm bu verileri
* tek bir paket halinde programa yani C#'a sunar.
********************************************************************************/

CREATE PROCEDURE sp_AdminPanelIstatistikleri
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Veritabanýndan çekeceðimiz sayýlarý ve isimleri geçici olarak hafýzada tutmak için deðiþkenler oluþturuyoruz.
    DECLARE @ToplamDoktor int           -- Toplam doktor sayýsýný tutacak kova
    DECLARE @ToplamHasta int            -- Toplam hasta sayýsýný tutacak kova
    DECLARE @ToplamRandevu int          -- Toplam randevu sayýsýný tutacak kova
    DECLARE @EnPopulerBrans varchar(50) -- En çok gidilen branþýn adýný tutacak kova
    
    -- 2. SAYIM ÝÞLEMLERÝ
    -- Tbl_Doktorlar tablosundaki tüm satýrlarý sayar ve sonucu deðiþkene atar.
    SELECT @ToplamDoktor = COUNT(*) FROM Tbl_Doktorlar
    
    -- Tbl_Hastalar tablosundaki kayýtlý hasta sayýsýný hesaplar.
    SELECT @ToplamHasta = COUNT(*) FROM Tbl_Hastalar
    
    -- Tbl_Randevular tablosundaki toplam randevu hareketini hesaplar.
    SELECT @ToplamRandevu = COUNT(*) FROM Tbl_Randevular
    
    -- 3. EN POPÜLER BRANÞI BULMA 
    -- Bu sorgu þu mantýkla çalýþýr:
    -- a) RandevuBrans'a göre gruplama yap (GROUP BY).
    -- b) Her grubun kaç adet olduðunu say (COUNT).
    -- c) Çoktan aza doðru sýrala (ORDER BY   ...   DESC).
    -- d) En tepedeki 1 tanesini al (TOP 1) ve deðiþkene ata.
    SELECT TOP 1 @EnPopulerBrans = RandevuBrans 
    FROM Tbl_Randevular 
    GROUP BY RandevuBrans 
    ORDER BY COUNT(*) DESC

    -- 4. SONUÇLARI DÖNDÜRME
    -- Hafýzada biriken deðiþken deðerlerini tek bir tablo satýrý gibi dýþarýya (C# tarafýna) veriyoruz.
    -- Bu sayede C# tarafýnda 4 kere veritabanýna gitmek yerine, tek seferde hepsini alýyoruz.
    SELECT 
        @ToplamDoktor AS DoktorSayisi,       -- Admin panelindeki Label1'e gidecek
        @ToplamHasta AS HastaSayisi,         -- Admin panelindeki Label2'ye gidecek
        @ToplamRandevu AS RandevuSayisi,     -- Admin panelindeki Label3'e gidecek
        
        -- ISNULL Kontrolü: Eðer veritabaný boþsa ve en popüler branþ bulunamazsa (NULL), 
        -- program hata vermesin diye 'Veri Yok' yazýsý döndürür.
        ISNULL(@EnPopulerBrans, 'Veri Yok') AS PopulerBrans
END;
GO






-- STORED PROCEDURE 8: Randevu Ýptal Etme
/********************************************************************************
* PROSEDÜR ADI: sp_RandevuIptalEt
* AMAÇ: Belirtilen randevuyu veritabanýndan güvenli bir þekilde silmek.
* MANTIK: 
* Doðrudan silme iþlemi yapmak yerine önce randevunun
* tarihini kontrol eder. Eðer randevu geçmiþte kalmýþsa silinmesini engeller.
* Bu sayede "Hangi hasta ne zaman gelmiþti?" bilgisinin kaybolmasýný önler.
********************************************************************************/

CREATE PROCEDURE sp_RandevuIptalEt
    -- GÝRÝÞ PARAMETRESÝ
    @RandevuID int  -- Silinmek istenen randevunun benzersiz kimlik numarasý yani ID'si.
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Randevunun tarihini veritabanýndan çekip hafýzada tutmak için geçici bir deðiþken tanýmladým.
    DECLARE @RandevuTarih date
    
    -- 2. BÝLGÝ ÇEKME
    -- Parametre olarak gelen ID'ye sahip randevuyu bul,
    -- o randevunun tarihini @RandevuTarih deðiþkenine ata.
    SELECT @RandevuTarih = RandevuTarih FROM Tbl_Randevular WHERE Randevuid = @RandevuID
    
    -- 3. TARÝH KONTROLÜ
    -- GETDATE(): O anki tarih ve saati verir.
    -- Eðer randevu tarihi bugünden küçükse yani geçmiþþ tarihse:
    IF @RandevuTarih < CAST(GETDATE() AS DATE)
    BEGIN
        -- HATA DURUMU 
        -- Geçmiþ randevular silinmemelidir çünkü onlar artýk birer "Arþiv" kaydýdýr.
        -- C# tarafýna '0' göndererek "Ýþlem Baþarýsýz" mesajý veriyoruz.
        RETURN 0; 
    END
    ELSE
    BEGIN
        -- 4. SÝLME ÝÞLEMÝ 
        -- Tarih bugün veya gelecek bir tarihse, randevu iptal edilebilir.
        DELETE FROM Tbl_Randevular WHERE Randevuid = @RandevuID
        
        -- C# tarafýna '1' göndererek "Ýþlem Baþarýlý" mesajý veriyoruz.
        RETURN 1; 
    END
END;
GO







-- STORED PROCEDURE 9: Doktor Þifre Güncelleme
/********************************************************************************
* PROSEDÜR ADI: sp_DoktorSifreGuncelle
* AMAÇ: Doktorun þifresini güvenli bir þekilde güncellemek.
* MANTIK: 
* Standart bir güncelleme iþleminden farklý olarak, doðrudan yeni þifreyi kaydetmez.
* Önce kullanýcýnýn girdiði "Eski Þifre"nin veritabanýndaki ile eþleþip eþleþmediðini
* kontrol eder. Bu sayede hesap güvenliði saðlanmýþ olur.
********************************************************************************/

CREATE PROCEDURE sp_DoktorSifreGuncelle
    -- GÝRÝÞ PARAMETRELERÝ
    @DoktorTC char(11),      -- Þifresini deðiþtirecek doktorun kimlik numarasý
    @EskiSifre varchar(10),  -- Doðrulama için doktorun girdiði mevcut yani eski þifre
    @YeniSifre varchar(10)   -- Veritabanýna kaydedilecek olan yeni þifre
AS
BEGIN
    -- 1. GÜVENLÝK DOÐRULAMASI
    -- Veritabanýnda; gönderilen TC numarasýna sahip ve þifresi girilen eski þifreyle 
    -- birebir ayný olan bir kayýt var mý?
    IF EXISTS (SELECT 1 FROM Tbl_Doktorlar WHERE DoktorTC = @DoktorTC AND DoktorSifre = @EskiSifre)
    BEGIN
        -- ---------------------------------------------------------
        -- DURUM 1: DOÐRULAMA BAÞARILI
        -- ---------------------------------------------------------
        -- Eski þifre doðru girildiyse güncelleme iþlemini yap.
        UPDATE Tbl_Doktorlar 
        SET DoktorSifre = @YeniSifre -- Sadece þifre alanýný yeni þifreyle deðiþtir
        WHERE DoktorTC = @DoktorTC;  -- Sadece ilgili doktorun kaydýný güncelle
        
        -- C# tarafýna '1' göndererek "Þifre Baþarýyla Deðiþtirildi" mesajý veriyoruz.
        RETURN 1; 
    END
    ELSE
    BEGIN
        -- ---------------------------------------------------------
        -- DURUM 2: DOÐRULAMA BAÞARISIZ 
        -- ---------------------------------------------------------
        -- Eðer TC doðru olsa bile Eski Þifre yanlýþ girildiyse bu bloða düþer.
        -- Hiçbir güncelleme yapmadan iþlemi bitiririz.
        
        -- C# tarafýna '0' göndererek "Eski Þifreniz Hatalý" mesajý veriyoruz.
        RETURN 0;
    END
END;
GO






-- STORED PROCEDURE 10: Hýzlý Hasta Arama Özelliði
/********************************************************************************
* PROSEDÜR ADI: sp_HastaArama
* AMAÇ: Tek bir kelime ile Ad, Soyad veya TC sütunlarýnda geniþ kapsamlý arama yapmak.
* MANTIK: 
* Kullanýcý arama kutusuna bir þey yazdýysa, bu kelimeyi tüm ilgili sütunlarda
* "içinde geçiyor mu?" (LIKE %...%) mantýðýyla arar.
* Eðer kutu boþ býrakýlýp butona basýldýysa, filtreyi kaldýrýr ve tüm listeyi getirir.
********************************************************************************/

CREATE PROCEDURE sp_HastaArama
    -- GÝRÝÞ PARAMETRESÝ
    @AranacakKelime varchar(50) -- Kullanýcýnýn textbox'a yazdýðý arama metni
AS
BEGIN
    -- 1. DURUM: KULLANICI HÝÇBÝR ÞEY YAZMADIYSA
    -- Parametre NULL gelirse VEYA boþluk ('') gelirse:
    IF @AranacakKelime IS NULL OR @AranacakKelime = ''
    BEGIN
        -- Filtreleme yapma, tablodaki bütün hastalarý listele.
        -- Bu sayede "Ara" butonu ayný zamanda "Listeyi Yenile" iþlevi görür.
        SELECT * FROM Tbl_Hastalar
    END
    -- 2. DURUM: KULLANICI BÝR KELÝME YAZDIYSA
    ELSE
    BEGIN
        -- Geniþ kapsamlý filtreleme yap.
        -- LIKE operatörü ve '%' karakteri kullanýlýr.
        -- '%ahmet%' demek, Ýçinde "ahmet" geçen her þeyi bul demektir.
        
        SELECT * FROM Tbl_Hastalar 
        WHERE 
            -- Girilen kelimeyi AD sütununda ara VEYA
            HastaAd LIKE '%' + @AranacakKelime + '%' OR 
            
            -- Girilen kelimeyi SOYAD sütununda ara VEYA
            HastaSoyad LIKE '%' + @AranacakKelime + '%' OR 
            
            -- Girilen kelimeyi TC sütununda ara.
            HastaTC LIKE '%' + @AranacakKelime + '%'
            
            -- 'OR' kullandýðýmýz için, kelime bu 3 alandan herhangi birinde geçse bile
            -- o kaydý sonuç olarak getirir.
    END
END;
GO



































