USE HastaneOtomasyonu;


/*********Branþlarý ve doktorlarý sayýlarý çok fazla olduðu için manuel olarak iþlemek yerine SQL sorgusu ile ekledim.************/


-- Örnek branþlarý ekledim.
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Dahiliye');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Göz Hastalýklarý');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Kardiyoloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Genel Cerrahi');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Göðüs Hastalýklarý');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Kulak Burun Boðaz');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Nöroloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Týbbi Onkoloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Üroloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Radyoloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Ortodonti');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Beyin Ve Sinir Cerrahisi');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Çocuk Cerrahisi');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Diþ Hekimliði');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Dermatoloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Periodontoloji');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Psikiyatri');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Fizik Tedavi');
INSERT INTO Tbl_Branslar (BransAd) VALUES ('Göðüs Cerrahisi');


-- Örnek doktorlarý ekledim.
INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ahmet', 'Yýlmaz', 'Dahiliye', '19356943295', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ayþe', 'Kaya', 'Göz Hastalýklarý', '59457805923', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ýpek', 'Genç', 'Periodontoloji', '19035459805', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Þaban', 'Kýlýç', 'Dermatoloji', '18032723590', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Mehmet', 'Demir', 'Kardiyoloji', '89535809458', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Tuncer', 'Suiçer', 'Dahiliye', '95499343004', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Serpil', 'Yön', 'Göðüs Cerrahisi', '26873402478', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Burak', 'Kütük', 'Fizik Tedavi', '20438043948', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Özgür', 'Bulut', 'Psikiyatri', '39302593457', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Hümeyra', 'Baþcý Ergül', 'Týbbi Onkoloji', '59238792348', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Canan', 'Ayman', 'Ortodonti', '73279239238', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Aysu', 'Þeker', 'Nöroloji', '24274828989', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Melike', 'Kaya', 'Göðüs Cerrahisi', '89543893203', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Adem', 'Utlu', 'Radyoloji', '73578729723', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ömer', 'Karter', 'Üroloji', '65743932485', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Zeynep', 'Yýlmaz', 'Fizik Tedavi', '39258923599', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Füsun', 'Akdeniz', 'Psikiyatri', '30825379236', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Hasan', 'Doðan', 'Çocuk Cerrahi', '39537203250', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Muhittin', 'Bodur', 'Periodontoloji', '23857832032', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Muhittin', 'Atar', 'Beyin Ve Sinir Cerrahisi', '18249042804', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Feyzullah', 'Çelik', 'Üroloji', '23057320034', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ceyda', 'Ak', 'Kardiyoloji', '35280239025', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Sadeddin', 'Kalkandelen', 'Göz Hastalýklarý', '12786573564', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Soner', 'Özcan', 'Kulak Burun Boðaz', '79783585612', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Gökhan', 'Bilen', 'Týbbi Onkoloji', '19397346153', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Burçin', 'Ýnce', 'Diþ Hekimliði', '16893495935', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Adem', 'Öztürk', 'Ortodonti', '49835679416', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Büþra', 'Kýlýç', 'Nöroloji', '83964791358', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Nur', 'Aydýn', 'Beyin Ve Sinir Cerrahisi', '67591759729', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Yanký', 'Boyacý', 'Psikiyatri', '42376591676', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Vicdan', 'Özkul', 'Genel Cerrahi', '46759117967', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ceren', 'Gül', 'Dermatoloji', '35795746561', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ýdris', 'Sayýlýr', 'Kardiyoloji', '19675493295', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Merve', 'Karabacak', 'Periodontoloji', '19675973764', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Alper', 'Arslan', 'Göðüs Cerrahisi', '16759249727', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Mustafa Can', 'Karabina', 'Genel Cerrahisi', '57953679467', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Mine', 'Ayvaz', 'Dahiliye', '57967435674', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Güney', 'Erdoðan', 'Týbbi Onkoloji', '63579551297', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Pýnar', 'Macit', 'Ortodonti', '35974626764', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Asuman', 'Kýzýl', 'Radyoloji', '25675972467', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Enfal', 'Sezer', 'Fizik Tedavi', '96725676147', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Kürþat', 'Akpýnar', 'Göz Hastalýklarý', '59757264627', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Bayram', 'Dalgalý', 'Beyin Ve Sinir Cerrahisi', '79656974647', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Elif', 'Çomaklý', 'Kulak Burun Boðaz', '76957467238', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Tuðçe', 'Yavaþ', 'Nöroloji', '76597466394', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Zeynep', 'Dursun', 'Genel Cerrahi', '72356759624', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Emine', 'Gürgür', 'Diþ Hekimliði', '36975724674', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Fatma', 'Ersözlü', 'Radyoloji', '97864269756', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Sema', 'Tekin', 'Çocuk Cerrahisi', '97685321467', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ebru', 'Akal', 'Dermatoloji', '97685263767', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Murat', 'Akarsu', 'Diþ Hekimliði', '26756374674', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Cahit', 'Demirtaþ', 'Çocuk Cerrahisi', '29765267267', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Ýlknur', 'Demir', 'Kulak Burun Boðaz', '97657264796', '1234');

INSERT INTO Tbl_Doktorlar (DoktorAd, DoktorSoyad, DoktorBrans, DoktorTC, DoktorSifre) 
VALUES ('Yasin', 'Ceyhan', 'Üroloji', '45276956746', '1234');
