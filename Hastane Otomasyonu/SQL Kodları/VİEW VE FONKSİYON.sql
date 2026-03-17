USE HastaneOtomasyonu;
GO



/******************************** 1 TANE VÝEW ***********************************/




/********************************************************************************
* VIEW ADI: View_RandevuDetay
* AMAÇ: Randevular ve Hastalar tablosunu birleþtirerek tek bir rapor oluþturmak.
* MANTIK: 
* Veritabanýnda randevular ve hastalar ayrý tablolarda tutulur.
* Ancak ekranda randevu listesini gösterirken "Hasta TC"nin yanýnda "Hasta Adý"ný da
* görmek isteriz. Bu View, iki tabloyu sanal olarak birleþtirir ve bize
* sanki tek bir tabloymuþ gibi sunar.
********************************************************************************/

CREATE VIEW View_RandevuDetay      
AS
SELECT 
    -- 1. RANDEVU BÝLGÝLERÝ (r kýsaltmasý Randevular tablosunu temsil eder)
    r.Randevuid,
    r.RandevuTarih,
    r.RandevuSaat,
    r.RandevuBrans,
    r.RandevuDoktor,
    r.RandevuDurum,
    
    -- 2. HASTA BÝLGÝLERÝ (h kýsaltmasý Hastalar tablosunu temsil eder)
    -- Randevu tablosunda sadece TC var, Ad ve Soyad yok.
    -- Bu bilgileri Hastalar tablosundan çekip yanýna ekliyoruz.
    r.HastaTC,          -- Ortak alan (Köprü)
    h.HastaAd,          -- Hastalar tablosundan geliyor
    h.HastaSoyad,       -- Hastalar tablosundan geliyor
    h.HastaTelefon,     -- Ýletiþim için gerekli
    r.HastaSikayet      -- Randevudaki þikayet notu

FROM Tbl_Randevular r   -- Ana Tablo: Randevular (Buna 'r' takma adýný verdik)

-- 3. TABLOLARI BÝRLEÞTÝRME
-- LEFT JOIN Mantýðý:
-- "Tbl_Randevular" tablosundaki tüm kayýtlarý getir.
-- Eðer o randevunun bir sahibi varsa git "Tbl_Hastalar"dan detaylarýný bul ve yanýna yaz.
-- Eðer randevu boþsa veya hasta bilgisi yoksa, hasta kýsýmlarýný boþ getir ama randevuyu yine de göster.
LEFT JOIN Tbl_Hastalar h ON r.HastaTC = h.HastaTC;

-- KULLANIM KOLAYLIÐI:
-- C# tarafýnda artýk:
-- "Select * From Tbl_Randevular Inner Join Tbl_Hastalar on....." diye uzun sorgu yazmaya gerek yok.
-- Sadece: "Select * From View_RandevuDetay" yazmak yeterlidir.


Select * From View_RandevuDetay

/**************************************** 6 TANE FONKSÝYON ******************************************/


                                                                                            --TARÝH:07/12/2025


--BURADAKÝ ÝLK ÜÇ FONKSÝYON DÝÐER 3 FONKSÝYONA GÖRE DAHA BASÝT DÜZEYDEDÝR.
--4, 5 VE 6 NUMARALI FONKSÝYONLAR DAHA SONRADAN YAZILMIÞ GELÝÞMÝÞ FONKSÝYONLARDIR. 





GO

--1.FONKSÝYON: Ýstenilen Branþtaki Doktor Sayýsýný Hesaplamak

/********************************************************************************
* FONKSÝYON ADI: fn_BransDoktorSayisi
* AMAÇ: Belirli bir týbbi branþta görev yapan doktor sayýsýný hesaplamak.
* MANTIK: 
* Bu fonksiyon, dýþarýdan bir "Branþ Adý" alýr.
* Doktorlar tablosuna gidip, sadece o branþa ait olan kayýtlarý sayar.
* Sonuç olarak bir sayý döndürür.
* Örnek Kullaným: Ýstatistik panellerinde veya doluluk oraný hesaplamalarýnda kullanýlýr.
********************************************************************************/

CREATE FUNCTION fn_BransDoktorSayisi
    -- GÝRÝÞ PARAMETRESÝ
    @BransAd varchar(30) -- Hangi branþýn sayýsýný istiyoruz? (Örn: 'Dahiliye')
RETURNS int -- Fonksiyonun geriye döndüreceði verinin tipi 
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Sayma iþleminin sonucunu geçici olarak tutacak bir deðiþken oluþturuyoruz.
    DECLARE @Sayi int

    -- 2. HESAPLAMA ÝÞLEMÝ
    -- Tbl_Doktorlar tablosuna git.
    -- DoktorBrans sütunu, parametre olarak gelen @BransAd ile eþleþenleri bul.
    -- COUNT(*) komutu ile bu eþleþenleri say ve sonucu @Sayi deðiþkenine ata.
    SELECT @Sayi = COUNT(*) FROM Tbl_Doktorlar WHERE DoktorBrans = @BransAd

    -- 3. SONUCU DÖNDÜRME
    -- Hesaplanan sayýyý fonksiyonu çaðýran yere gönder.
    RETURN @Sayi
END;


-- ------------------------------------------------------------------
-- ÖRNEK KULLANIM 
-- ------------------------------------------------------------------
-- SQL Server'da kullanýcý tanýmlý fonksiyonlar çaðrýlýrken 
-- baþýna mutlaka þema adý olan "dbo." (Database Owner) eklenmelidir.
-- Bu sorgu, 'Dahiliye' branþýndaki toplam doktor sayýsýný tek bir rakam olarak getirir.

SELECT dbo.fn_BransDoktorSayisi('Dahiliye') AS DahiliyeDoktorSayisi



GO











--2.FONKSÝYON: Ad Soyad Birleþtirme

/********************************************************************************
* FONKSÝYON ADI: fn_AdSoyadBirlestir
* AMAÇ: Ayrý sütunlarda tutulan Ad ve Soyad bilgilerini tek formatta birleþtirmek.
* MANTIK: 
* Veritabaný tasarýmý gereði isim ve soyisim ayrý ayrý saklanýr.
* Ancak arayüzde (Doktor Listesi, Randevu Ekraný vb.) bunlar genellikle bitiþik istenir.
* Bu fonksiyon, iki metni alýp araya boþluk koyarak yapýþtýrýr ve tek parça döndürür.
********************************************************************************/

CREATE FUNCTION fn_AdSoyadBirlestir
    -- GÝRÝÞ PARAMETRELERÝ
    @Ad varchar(20),    -- Tablodan gelen isim verisi
    @Soyad varchar(20)  -- Tablodan gelen soyisim verisi
RETURNS varchar(41)    
AS
BEGIN
    -- ÝÞLEM VE DÖNDÜRME
    -- SQL'de '+' operatörü metinleri yan yana eklemek için kullanýlýr.
    -- Araya týrnak içinde boþluk (' ') koyarak ismin düzgün görünmesini saðlýyoruz.
    
    RETURN @Ad + ' ' + @Soyad
END;
GO

-- ------------------------------------------------------------------
-- ÖRNEK KULLANIM
-- ------------------------------------------------------------------
-- Bu fonksiyon sayesinde sorgularýmýzda sürekli (Ad + ' ' + Soyad) yazmak zorunda kalmayýz.
-- Ayrýca yarýn öbür gün formatý (SOYAD, Ad) þeklinde deðiþtirmek istersek,
-- sadece burayý deðiþtirmemiz tüm projeyi günceller.

SELECT dbo.fn_AdSoyadBirlestir('Ahmet', 'Yýlmaz') AS DoktorAdSoyad
 











GO
--3.FONKSÝYON: Bir doktorun toplam kaç tane randevusu varsa o sayýyý geitrir.

/********************************************************************************
* FONKSÝYON ADI: fn_DoktorRandevuSayisi
* AMAÇ: Belirli bir doktorun toplam randevu sayýsýný hesaplamak.
* MANTIK: 
* Dýþarýdan bir doktor adý alýr.
* Randevular tablosunu tarayarak, bu ismin randevu listesinde kaç kez geçtiðini sayar.
* Sonuç olarak o doktorun bakacaðý toplam hasta sayýsýný verir.
********************************************************************************/

CREATE FUNCTION fn_DoktorRandevuSayisi
    -- GÝRÝÞ PARAMETRESÝ
    @DoktorAd varchar(20) -- Randevu sayýsýný öðrenmek istediðimiz doktorun adý
RETURNS int -- Toplam randevu adedi
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Sayma iþleminin sonucunu tutacak geçici bir deðiþken.
    DECLARE @Toplam int

    -- 2. HESAPLAMA ÝÞLEMÝ
    -- Tbl_Randevular tablosuna git.
    -- 'RandevuDoktor' sütunu, parametre olarak gelen @DoktorAd ile ayný olanlarý bul.
    -- Bunlarýn hepsini say ve sonucu @Toplam deðiþkenine aktar.
    SELECT @Toplam = COUNT(*) FROM Tbl_Randevular WHERE RandevuDoktor = @DoktorAd

    -- 3. SONUCU DÖNDÜRME
    -- Hesaplanan toplam sayýyý fonksiyonu kullanan yere geri gönder.
    RETURN @Toplam
END;
GO

-- ------------------------------------------------------------------
-- ÖRNEK KULLANIM
-- ------------------------------------------------------------------
-- Bu sorgu sayesinde karmaþýk Count iþlemlerine girmeden, 
-- sadece isim vererek o doktorun yoðunluðunu görebiliriz.

SELECT dbo.fn_DoktorRandevuSayisi('Ahmet Yýlmaz') AS RandevuSayýsý






                                                                                                 --TARÝH:18/12/2025






GO
--4.FONKSÝYON: T.C. Kimlik Numarasýnýn Ortadaki 5 Hanesini Gizleme

/********************************************************************************
* FONKSÝYON ADI: fn_TCMaskele
* AMAÇ: Kiþisel verilerin gizliliði için TC Kimlik Numarasýný maskelemek.
* MANTIK: 
* TC Numarasý 11 hanelidir. Tamamýný göstermek yerine;
* Ýlk 3 haneyi gösterir, aradaki 5 haneyi yýldýz (*) ile kapatýr, son 3 haneyi gösterir.
* Böylece hem güvenlik saðlanýr hem de kiþi "Bu benim TC kimlik numaram" diyebilir.
* Örnek: 12345678901 -> 123*****901 þeklinde dönüþtürür.
********************************************************************************/

CREATE FUNCTION fn_TCMaskele
    -- GÝRÝÞ PARAMETRESÝ
    @TC char(11) -- Maskelenecek olan 11 haneli orijinal TC kimlik numarasý
RETURNS varchar(11) -- Maskelenmiþ hali
AS
BEGIN
    -- 1. DOÐRULAMA
    -- Gelen verinin 11 hane olduðunu kontrol ediyoruz.
    -- Eðer veri eksik veya boþsa maskeleme yapma, hata mesajý döndür.
    IF LEN(@TC) <> 11
        RETURN 'Hatalý TC'

    -- 2. MASKELEME ÝÞLEMÝ
    -- LEFT(@TC, 3)  : TC'nin solundan yani baþýndan ilk 3 rakamý al.
    -- '*****'       : Araya 5 tane yýldýz koy.
    -- RIGHT(@TC, 3) : TC'nin saðýndan yani sonundan son 3 rakamý al.
    
    -- Yani 3 (Baþ) + 5 (Yýldýz) + 3 (Son) = 11 Karakter.
    -- Böylece verinin uzunluðu bozulmaz, orijinali gibi 11 hane görünür.

    RETURN LEFT(@TC, 3) + '*****' + RIGHT(@TC, 3)
END;
GO

-- ------------------------------------------------------------------
-- ÖRNEK KULLANIM
-- ------------------------------------------------------------------
-- Bu sorgu, hasta listesini çekerken TC numaralarýný gizleyerek getirir.
-- Sekreter ekranýnda veya doktor ekranýnda hastalarý listelerken kullanýlýr.

SELECT HastaAd, dbo.fn_TCMaskele(HastaTC) as GizliTC FROM Tbl_Hastalar





GO
--5.FONKSÝYON: Doktor Yoðunluðu Bulma

/********************************************************************************
* FONKSÝYON ADI: fn_DoktorGunlukYogunluk
* AMAÇ: Doktorlarýn anlýk iþ yükünü analiz edip sözel bir durum raporu vermek.
* MANTIK: 
* Sadece "Bugün 12 randevusu var" demek yerine, bu sayýnýn ne anlama geldiðini söyler.
* Veritabanýndaki sayýyý alýr, belli kriterlere göre sýnýflandýrýr.
* Sonuç olarak "Müsait", "Normal" veya "Çok Yoðun" gibi anlaþýlýr bir metin döndürür.
********************************************************************************/

CREATE FUNCTION fn_DoktorGunlukYogunluk
    -- GÝRÝÞ PARAMETRESÝ
    @DoktorAd varchar(50) -- Durumu kontrol edilecek doktorun Adý ve Soyadý
RETURNS varchar(20) -- Durum metni (Örn: 'Çok Yoðun')
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    DECLARE @RandevuSayisi int  -- Doktorun randevu sayýsýný tutacak deðiþken
    DECLARE @Durum varchar(20)  -- Sonuç metnini tutacak deðiþken

    -- 2. VERÝ ANALÝZÝ
    -- Doktorun sadece bugünkü randevularýný saymamýz lazým.
    -- CAST(GETDATE() AS DATE): Saati atýp sadece bugünün tarihini (Gün/Ay/Yýl) alýr.
    -- Böylece geçmiþ veya gelecek randevular hesaba katýlmaz, anlýk durum görülür.
    
    SELECT @RandevuSayisi = COUNT(*) FROM Tbl_Randevular 
    WHERE RandevuDoktor = @DoktorAd AND RandevuTarih = CAST(GETDATE() AS DATE)

    -- 3. KARAR MEKANÝZMASI
    -- Sayýya göre bir etiket belirler.
    
    SET @Durum = CASE 
                    -- Eðer randevu sayýsý 5'ten azsa:
                    WHEN @RandevuSayisi < 5 THEN 'Müsait (Boþ)'
                    
                    -- Eðer randevu sayýsý 5 ile 10 arasýndaysa:
                    WHEN @RandevuSayisi BETWEEN 5 AND 10 THEN 'Normal'
                    
                    -- Eðer randevu sayýsý 10'dan fazlaysa:
                    WHEN @RandevuSayisi > 10 THEN 'Çok Yoðun'
                 END

    -- 4. SONUCU DÖNDÜRME
    -- Belirlenen durum metnini fonksiyonu çaðýran yere gönder.
    RETURN @Durum
END;
GO

-- ------------------------------------------------------------------
-- ÖRNEK KULLANIM
-- ------------------------------------------------------------------
-- Bu sorgu, doktor listesini çekerken yanýna bugünkü yoðunluk durumunu da yazar.
-- Sekreter, hangi doktorun boþ olduðunu tek bakýþta görebilir.

SELECT DoktorAd, DoktorSoyad, dbo.fn_DoktorGunlukYogunluk(DoktorAd + ' ' + DoktorSoyad) as Durum 
FROM Tbl_Doktorlar








GO
--6.FONKSÝYON: Radevu Detaylarý

/********************************************************************************
* FONKSÝYON ADI: fn_RandevuDetayliDurum
* AMAÇ: Randevunun sadece "Dolu/Boþ" bilgisini deðil, güncel durumunu metin olarak vermek.
* MANTIK: 
* Veritabanýnda durum sadece 1 veya 0 olarak tutulur. Ancak gerçek hayatta 3 durum vardýr:
* 1. Müsait (0)
* 2. Aktif Randevu (1 ve Tarihi Gelmemiþ)
* 3. Geçmiþ Randevu (1 ama Tarihi Geçmiþ)
* Bu fonksiyon, tarihi de hesaba katarak bize net bir durum raporu verir.
********************************************************************************/

CREATE FUNCTION fn_RandevuDetayliDurum
    -- GÝRÝÞ PARAMETRELERÝ
    @Durum bit,   -- Randevunun veritabanýndaki ham durumu (0: Boþ, 1: Dolu)
    @Tarih date   -- Randevunun tarihi (Bugün mü, geçmiþ mi kontrolü için)
RETURNS varchar(30) -- Anlaþýlýr durum metni (Örn: 'Tamamlandý')
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Sonuç metnini geçici olarak tutacak deðiþken.
    DECLARE @Sonuc varchar(30)

    -- 2. DURUM ANALÝZÝ
    
    -- a-) Randevu hiç alýnmamýþsa (Durum = 0)
    IF @Durum = 0
    BEGIN
        SET @Sonuc = 'Müsait' -- Bu saat dilimi boþtur, randevu alýnabilir.
    END
    
    -- b-) Randevu alýnmýþsa (Durum = 1)
    ELSE
    BEGIN
        -- Randevu alýnmýþ ama tarihi geçmiþ mi yoksa hala aktif mi?
        
        -- GETDATE(): Þu anki zamaný verir.
        -- CAST(... AS DATE): Saati atar, sadece tarihi alýr.
        
        -- Eðer randevu tarihi bugünden KÜÇÜKSE yani geçmiþþse:
        IF @Tarih < CAST(GETDATE() AS DATE)
        BEGIN
            -- Randevu dolu görünse bile tarihi geçtiði için artýk "Bitmiþ" kabul edilir.
            SET @Sonuc = 'Tamamlandý (Geçmiþ)'
        END
        
        -- Eðer randevu tarihi bugün veya gelecekteyse:
        ELSE
        BEGIN
            -- Randevu hala geçerlidir ve o saat doludur.
            SET @Sonuc = 'Dolu (Aktif)'
        END
    END

    -- 3. SONUCU DÖNDÜRME
    -- Hesaplanan metni fonksiyonu çaðýran yere yani C# DataGridView'ine gönder.
    RETURN @Sonuc
END;
GO












