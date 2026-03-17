

/*************************TRIGGER'LAR*************************/


--Otomasyonda gerçekleþen bazý önemli hareketler için trigger kullanýldý.
--Hareketleri kontrol etmeyi C# tarafýnda yapsaydýk çok fazla kod yazmam gerekecekti.


USE HastaneOtomasyonu;
GO


-- Ýlk önce Hareketleri tutacak olan tabloyu oluþturduk.
CREATE TABLE Tbl_Hareketler (
    Hareketid int PRIMARY KEY IDENTITY(1,1),        --Hareketid'sini 1'den baþlatarak 1'er 1'er artýrýr.
    Islem varchar(100),                             --Ýþlemi tabloya ekler.
    Tarih datetime DEFAULT GETDATE()                --Ýþlemin yapýldýðý tarihi tabloya ekler.
);
GO






                                                                                            --TARÝH:05/12/2025
    

--1.TRIGGER: Yeni Hasta Kaydý Ýþlemini Hareketler Tablosuna Kaydetmek

/********************************************************************************
* TRIGGER ADI: trg_HastaKayit
* AMAÇ: Yeni hasta kayýtlarýný otomatik olarak takip etmek.
* MANTIK: 
* Bu tetikleyici, bir gözcü gibi çalýþýr. Gözü sürekli 'Tbl_Hastalar' tablosundadýr.
* Ne zaman bu tabloya yeni bir satýr eklense (INSERT INTO komutu ile), tetikleyici hemen devreye girer
* ve 'Tbl_Hareketler' tablosuna gidip gerçekleþen iþlemi buraya kaydeder.
********************************************************************************/

CREATE TRIGGER trg_HastaKayit
    ON Tbl_Hastalar -- Bu trigger sadece Hastalar tablosunu izler.
    AFTER INSERT    -- Ekleme iþlemi baþarýyla bittikten hemne sonra çalýþýr.
AS
BEGIN

    -- Sekreter hasta ekle butonuna bastýðýnda C# sadece hastayý kaydeder.
    -- Ancak bu trigger arka planda sessizce çalýþýr ve Hareketler tablosuna "Yeni bir hasta kaydý oluþturuldu." measajýný kaydeder.
    
    INSERT INTO Tbl_Hareketler (Islem) 
    VALUES ('Yeni bir hasta kaydý oluþturuldu.')
    
    -- NOT: Tbl_Hareketler tablosunda genellikle bir de 'Tarih' sütunu olur 
    -- ve varsayýlan deðeri (Default Value) 'GetDate()' olduðu için 
    -- buraya tarih yazmasak bile o anki zaman otomatik kaydedilir.
END;
GO

-- NOT: Tbl_Hareketler tablosundaki Tarih sütunu direkt o anki tarih varsayýlan olarak alýnýr.(GETDATE() ile)






--2.TRIGGER: Alýnan Randevularý Hareketler Tablosuna Kaydetmek

/********************************************************************************
* TRIGGER ADI: trg_RandevuAl
* AMAÇ: Randevu iþlemlerinin sistem tarafýndan otomatik günlüðünü tutmak.
* MANTIK: 
* Bu tetikleyici, 'Tbl_Randevular' tablosuna bekçilik yapar.
* Hasta veya Sekreter sisteme yeni bir randevu kaydettiðinde, 
* tetikleyici bunu fark eder ve 'Tbl_Hareketler' tablosuna 
* "Yeni bir randevu oluþturuldu" measjýný kaydeder.
********************************************************************************/

CREATE TRIGGER trg_RandevuAl
    ON Tbl_Randevular -- Tetikleyici sadece Randevular tablosunu takip eder.
    AFTER INSERT      -- Kayýt iþlemi baþarýyla bittikten sonra çalýþýr.
AS
BEGIN

    -- Kullanýcý randevu al butonuna bastýðýnda aslýnda sadece randevu tablosuna kayýt yapar.
    -- Ancak biz arka planda bu iþlemi Hareketler tablosuna da iþleriz.
    -- Bu sayede Admin panelinde "Bugün kaç randevu alýndý?" sorusunun cevabýný görebiliriz.
    
    INSERT INTO Tbl_Hareketler (Islem) 
    VALUES ('Yeni bir randevu oluþturuldu.')
END;
GO







--3.TRIGGER: Doktor Silme Ýþlemini Hareketler Tablosuna Kaydetmek

/********************************************************************************
* TRIGGER ADI: trg_DoktorSil
* AMAÇ: Kritik veri silme iþlemlerini güvenlik amacýyla kayýt altýna almak.
* MANTIK: 
* Bir yönetici veya sekreter 'Tbl_Doktorlar' tablosundan bir doktoru sildiðinde,
* bu tetikleyici devreye girer. Silinen doktorun geri gelmeyeceðini bildiði için
* 'Tbl_Hareketler' tablosuna "Bir doktor silindi" mesajýný kaydeder.
* Bu sayede Doktor listesinden kim eksildi? sorusunun takibi yapýlýr.
********************************************************************************/

CREATE TRIGGER trg_DoktorSil
    ON Tbl_Doktorlar -- Tetikleyici Doktorlar tablosundaki eksilmeleri takip eder.
    AFTER DELETE     -- Silme iþlemi baþarýyla tamamlandýktan sonra çalýþýr.
AS
BEGIN
    -- Veritabanýndan bir doktor kaydý fiziksel olarak silindiðinde,
    -- bu olay güvenlik açýsýndan kritik olduðu için Hareketler tablosuna iþlenir.
    -- Bu iþlem, sistemin "Güvenlik Günlüðü"nü (Audit Log) oluþturur.
    
    INSERT INTO Tbl_Hareketler (Islem) 
    VALUES ('Sistemden bir doktor kaydý silindi.')
END;
GO







--4.TRIGGER: Branþ Ekleme Ýþlemini Hareketler Tablosuna Kaydetmek

/********************************************************************************
* TRIGGER ADI: trg_BransEkle
* AMAÇ: Hastane hizmet yapýsýndaki deðiþiklikleri yeni branþ eklemeyi izlemek.
* MANTIK: 
* Hastane yönetimi hizmet aðýný geniþletip yeni bir branþ 
* eklediðinde, bu tetikleyici devreye girer. 
* Yapýlan bu yapýsal deðiþikliði 'Tbl_Hareketler' tablosuna kaydeder.
********************************************************************************/

CREATE TRIGGER trg_BransEkle
    ON Tbl_Branslar -- Tetikleyici sadece Branþlar tablosunu dinler.
    AFTER INSERT    -- Yeni bir branþ baþarýyla eklendikten sonra çalýþýr.
AS
BEGIN

    -- Admin panelinden "Branþ Ekle" denildiðinde, hastaneye yeni bir týbbi birim kazandýrýlmýþ olur.
    -- Bu önemli bir idari iþlemdir. Sistem yöneticisinin ne zaman yeni bir birim açtýðýný 
    -- takip etmek için Hareketler tablosuna kayýt yapýlýr.
    
    INSERT INTO Tbl_Hareketler (Islem) 
    VALUES ('Yeni bir branþ eklendi.')
END;
GO




                                                                                            --TARÝH:25/12/2025
--BU TRIGGGER LAR ÖNCEKÝ TRIGGERLARIN GELÝÞMÝÞ HALÝDÝR...
--ÜSTTEKÝLERÝ SÝLMEYLE ZAMAN KAYBETMEK ÝSTEMEDÝM.


--5.TRIGGER: Randevusu Olan Doktorlarý Silmeyi Engellemek

/********************************************************************************
* TRIGGER ADI: trg_DoktorSilinemez
* TÜRÜ: INSTEAD OF DELETE (Silme Ýþlemi Yerine Geçen Tetikleyici)
* AMAÇ: Yanlýþlýkla veri silinmesini ve randevularýn boþa düþmesini engellemek.
* MANTIK: 
* Standart 'DELETE' komutu gönderildiðinde bu trigger araya girer.
* Silme iþlemini hemen YAPMAZ. Önce kontrol eder:
* "Bu doktorun gelecekte bakmasý gereken hastalar var mý?"
* VARSA -> Hata verir, iþlemi iptal eder.
* YOKSA -> Silme iþlemini bizzat kendisi gerçekleþtirir ve kaydýný tutar.
********************************************************************************/

CREATE TRIGGER trg_DoktorSilinemez
    ON Tbl_Doktorlar -- Doktorlar tablosu üzerinde çalýþýr.
    INSTEAD OF DELETE -- Silme iþlemi yapýlacaðý zaman, o iþlemin yerine bu kodlar çalýþacak.
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    DECLARE @DoktorAdSoyad varchar(50)
    DECLARE @DoktorTC char(11)
    
    -- 2. SÝLÝNMEK ÝSTENEN VERÝYÝ YAKALAMA
    -- SQL Server'da silinmeye çalýþýlan veriler geçici olarak 'deleted' adlý sanal bir tabloda tutulur.
    -- Biz silinmeye çalýþýlan doktorun bilgilerini oradan çekip hafýzaya alýyoruz.
    SELECT @DoktorTC = DoktorTC, @DoktorAdSoyad = (DoktorAd + ' ' + DoktorSoyad) FROM deleted;

    -- 3. KONTROL 
    -- Randevular tablosuna bak: Bu doktorun adý geçiyor mu? VE tarihi bugünden ileri mi?
    IF EXISTS (SELECT 1 FROM Tbl_Randevular WHERE RandevuDoktor = @DoktorAdSoyad AND RandevuTarih >= CAST(GETDATE() AS DATE))
    BEGIN

        
        -- C# tarafýna hata gönder. (16: Hata seviyesi, 1: Durum kodunu temsil edre.)
        RAISERROR('DÝKKAT: Bu doktorun ileri tarihli randevularý bulunmaktadýr. Önce randevularý iptal etmelisiniz, doktor silinemez!', 16, 1);
        
        -- Ýþlemi tamamen geri al. Silme iþlemi gerçekleþmedi.
        ROLLBACK TRANSACTION;
    END
    ELSE
    BEGIN
        -- DURUM: DOKTOR MÜSAÝT
        
        -- Bu trigger "INSTEAD OF" (Yerine geçen) olduðu için, normal DELETE iþlemi iptal olmuþtu.
        -- O yüzden silme kodunu burada bizim elle yazmamýz gerekir.
        DELETE FROM Tbl_Doktorlar WHERE DoktorTC = @DoktorTC;
        
        -- Silme iþlemi yapýldýðýna göre, bunu hareket tablosuna güvenle kaydedebiliriz.
        INSERT INTO Tbl_Hareketler (Islem, Tarih) 
        VALUES (@DoktorAdSoyad + ' isimli doktor silindi.', GETDATE());
    END
END;
GO




--6.TRIGGER: Detaylý Branþ Güncelleme

/********************************************************************************
* TRIGGER ADI: trg_BransGuncelleme
* AMAÇ: Branþ adý deðiþtiðinde iliþkili tablolardaki verileri otomatik güncellemek.
* MANTIK: 
* Normalde branþ adý deðiþirse, o branþa kayýtlý doktorlar ve randevular boþa düþer.
* Örneðin: "Göz" branþýnýn adýný "Göz Hastalýklarý" yaparsan, doktorlarýn branþý hala "Göz" kalýr.
* Bu trigger, ana tablodaki deðiþiklik olduðu an Doktorlar ve Randevular tablosuna gidip
* eski ismi bulur ve yeni isimle deðiþtirir.
********************************************************************************/

CREATE TRIGGER trg_BransGuncelleme
    ON Tbl_Branslar -- Branþlar tablosundaki deðiþiklikleri takip eder.
    AFTER UPDATE    -- Bir güncelleme iþlemi yapýldýktan sonra çalýþýr.
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    DECLARE @EskiAd varchar(30) -- Deðiþiklikten önceki eski ismi tutacak
    DECLARE @YeniAd varchar(30) -- Deðiþiklikten sonraki yeni ismi tutacak

    -- 2. ESKÝ VE YENÝ VERÝYÝ YAKALAMA
    -- SQL Server güncelleme sýrasýnda arka planda iki sanal tablo oluþturur:
    -- "deleted" tablosu: Verinin deðiþtirilmeden önceki halini tutar.
    -- "inserted" tablosu: Verinin yeni halini tutar.
    
    SELECT @EskiAd = BransAd FROM deleted;  -- Eski ismi al
    SELECT @YeniAd = BransAd FROM inserted; -- Yeni ismi al

    -- 3. KONTROL: GERÇEKTEN ÝSÝM MÝ DEÐÝÞTÝ?
    -- Belki kullanýcý sadece branþýn ID'sini veya baþka bir alanýný güncelledi, ismi deðiþtirmedi.
    -- Eðer eski isim ile yeni isim birbirinden farklýysa iþlem yap:
    IF @EskiAd <> @YeniAd
    BEGIN
        -- 4. OTOMATÝK DÜZELTME 
        
        -- A) Doktorlar Tablosunu Güncelle:
        -- Branþý eski isim olan tüm doktorlarý bul, onlarýn branþýný yeni isim yap.
        UPDATE Tbl_Doktorlar 
        SET DoktorBrans = @YeniAd 
        WHERE DoktorBrans = @EskiAd;

        -- B) Randevular Tablosunu Güncelle:
        -- Branþý eski isim olan tüm randevularý bul, onlarý da yeni isimle güncelle.
        UPDATE Tbl_Randevular 
        SET RandevuBrans = @YeniAd 
        WHERE RandevuBrans = @EskiAd;
        
        -- 5. KAYIT ALTINA ALMA
        -- Yapýlan bu deðþikliði Hareketler tablosuna kaydet.
        INSERT INTO Tbl_Hareketler (Islem, Tarih) 
        VALUES ('Branþ adý güncellendi: ' + @EskiAd + ' -> ' + @YeniAd, GETDATE());
    END
END;
GO





--7.TRIGGER: Hasta Bilgisi Güncellemelerini Takip Etmek

/********************************************************************************
* TRIGGER ADI: trg_HastaGuncellemeLog
* AMAÇ: Hasta bilgilerindeki deðiþiklikleri detaylý olarak takip etmek.
* MANTIK: 
* Standart bir güncelleme iþleminde eski veri silinir, yerine yenisi yazýlýr.
* Ancak biz eski veriyi kaybetmek istemiyoruz.
* Bu trigger, güncelleme anýnda "Eski Veri" ve "Yeni Veri"yi karþýlaþtýrýr.
* Eðer telefon numarasý deðiþmiþse, eski ve yeni halini Hareketler tablosuna not eder.
********************************************************************************/

CREATE TRIGGER trg_HastaGuncellemeLog
    ON Tbl_Hastalar -- Hastalar tablosundaki güncellemeleri takip eder.
    AFTER UPDATE    -- Bilgiler güncellendikten sonra çalýþýr.
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Eski ve yeni verileri kýyaslamak için hafýzada tutacaðýmýz deðiþkenler.
    DECLARE @HastaAdSoyad varchar(50)
    DECLARE @EskiTel varchar(15)
    DECLARE @YeniTel varchar(15)

    -- 2. VERÝLERÝ YAKALAMA
    -- SQL Server güncelleme sýrasýnda iki sanal tablo kullanýr:
    -- "deleted": Güncelleme yapýlmadan önceki ESKÝ verileri tutar.
    -- ""inserted": Güncelleme yapýldýktan sonraki yeni verileri tutar.

    -- Eski telefon numarasýný ve hastanýn adýný "deleted" tablosundan alýyoruz.
    SELECT @HastaAdSoyad = (HastaAd + ' ' + HastaSoyad), @EskiTel = HastaTelefon FROM deleted;
    
    -- Yeni telefon numarasýný "inserted" tablosundan alýyoruz.
    SELECT @YeniTel = HastaTelefon FROM inserted;

    -- 3. KARÞILAÞTIRMA
    -- Kullanýcý belki sadece þifresini deðiþtirdi, telefonunu deðiþtirmedi.
    -- Eðer telefon numarasý gerçekten deðiþmiþse kayýt tutacaðýz.
    IF @EskiTel <> @YeniTel
    BEGIN
        -- 4. Kayýt Altýna Alma
        -- Deðiþikliði detaylý bir cümle halinde Hareketler tablosuna ekliyoruz.
        -- Mesela Þu Þekilde Bir Kayýt Olacak: "Ahmet Yýlmaz telefonunu deðiþtirdi. Eski: 555-111 Yeni: 555-222"
        
        INSERT INTO Tbl_Hareketler (Islem, Tarih)
        VALUES (@HastaAdSoyad + ' telefonunu deðiþtirdi. Eski: ' + @EskiTel + ' Yeni: ' + @YeniTel, GETDATE());
    END
END;
GO






--8.TRIGGER: Geçmiþ Tarihli Randevularý Güncellemeyi Engelleme

/********************************************************************************
* TRIGGER ADI: trg_TarihKontrol
* AMAÇ: Randevu tarihlerinin geçmiþe dönük güncellenmesini engellemek.
* MANTIK: 
* Bir randevunun tarihi deðiþtirilmek istendiðinde devreye girer.
* Kullanýcýnýn girdiði "Yeni Tarih"i kontrol eder.
* Eðer yeni tarih bugünden daha eski bir tarihse, "Geçmiþe randevu veremezsin" diyerek
* iþlemi iptal eder ve veriyi eski haline döndürür.
********************************************************************************/

CREATE TRIGGER trg_TarihKontrol
    ON Tbl_Randevular -- Randevular tablosu üzerindeki deðiþiklikleri denetler.
    AFTER UPDATE      -- Güncelleme iþlemi yapýldýðý an çalýþýr.
AS
BEGIN
    -- 1. DEÐÝÞKEN TANIMLAMA
    -- Kullanýcýnýn girdiði yeni tarihi hafýzada tutmak için deðiþken.
    DECLARE @YeniTarih date
    
    -- 2. YENÝ VERÝYÝ YAKALAMA 
    -- SQL Server, güncelleme yapýldýðýnda yeni verileri "inserted" adlý sanal tabloda tutar.
    -- Biz de kullanýcýnýn girdiði o yeni tarihi buradan çekip deðiþkenimize atýyoruz.
    SELECT @YeniTarih = RandevuTarih FROM inserted;

    -- 3. ZAMAN KONTROLÜ
    -- GETDATE(): O anki tarihi ve saati verir.
    -- CAST(... AS DATE): Saati atýp sadece gün/ay/yýl olarak kýyaslama yapmak için kullanýlýr.
    
    -- Kural: Eðer girilen yeni tarih, bugünün tarihinden KÜÇÜKSE yani geçmiþse:
    IF @YeniTarih < CAST(GETDATE() AS DATE)
    BEGIN
        -- 4. HATA MESAJI VE ÝPTAL ETME
        
        -- C# tarafýna özel bir hata mesajý gönder (Severity 16: Standart kullanýcý hatasý).
        RAISERROR('HATA: Randevu tarihi geçmiþ bir tarihe güncellenemez!', 16, 1);
        
        -- ÝÞLEMÝ GERÝ AL:
        -- Burasý çok önemlidir. Eðer ROLLBACK demezsek, SQL hatayý gösterir AMA kaydý da günceller.
        -- ROLLBACK TRANSACTION komutu, yapýlan o hatalý güncellemeyi tamamen iptal eder 
        -- ve veriyi güncelleme yapýlmadan önceki eski haline geri döndürür.
        ROLLBACK TRANSACTION; 
    END
END;
GO




















