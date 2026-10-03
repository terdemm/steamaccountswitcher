# 🎮 Steam Account Switcher Pro

[![Release](https://img.shields.io/badge/Release-v2.0.0-blue.svg)](https://github.com/terdemm/steamaccountswitcher/releases/tag/v2.0.0)
[![Download EXE](https://img.shields.io/badge/Download-SteamAccountSwitcher.exe-brightgreen.svg)](https://github.com/terdemm/steamaccountswitcher/releases/download/v2.0.0/SteamAccountSwitcher.exe)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011-0078d7.svg)]()
[![.NET](https://img.shields.io/badge/.NET-8.0%20WPF-512bd4.svg)]()
[![Portable](https://img.shields.io/badge/Single--File-Portable%20EXE-brightgreen.svg)]()
[![Credits](https://img.shields.io/badge/Based%20On-sahin--a%2FSteamAccountSwitcher-orange.svg)](https://github.com/sahin-a/SteamAccountSwitcher)

> **English:** Next-generation, lightning-fast, and elegant Steam Account Switcher & Launcher with a modern dark UI, custom tags, game auto-launch, quick power tools, and system tray integration. Delivered as a standalone **Single-File Portable Executable (.exe)**.
>
> **Türkçe:** Windows 10 & 11 için optimize edilmiş, modern karanlık arayüze, özel not/etiketleme sistemine, hızlı oyun başlatıcıya ve sistem tepsisi desteğine sahip yeni nesil açık kaynaklı **Steam Hesap Değiştirici ve Hızlı Başlatıcı**.

---

## 🌟 Acknowledgments & Credits

This project was built upon and inspired by the foundational concepts of the original open-source [sahin-a/SteamAccountSwitcher](https://github.com/sahin-a/SteamAccountSwitcher) repository. We express our gratitude to **sahin-a** and all contributors for providing the initial Steam VDF parsing and registry switching research. This project completely modernizes the architecture into a rich, self-contained single-file `.exe` with native WPF hardware acceleration, custom account notes, fast app launching, and system tray management.

---

# 🇬🇧 English Documentation

### 🚀 Key Features

* **⚡ Instant 1-Click Switching:** Seamlessly switch between any saved Steam account. Steam is closed gracefully, the auto-login configuration is updated, and Steam is restarted with your chosen profile.
* **🎨 Modern Hardware-Accelerated UI:** Sleek, responsive dark theme crafted with native WPF, custom typography, subtle glassmorphism borders, and animated status badges.
* **🏷️ Custom Notes, Tags & Color Badges:** Assign personalized notes (e.g., *"Main Account"*, *"CS2 Prime"*, *"Trading / Storage"*) and custom colored tags to easily organize your accounts.
* **⭐ Pinned Favorites:** Star and pin your most-used accounts to the top of your list.
* **🔍 Instant Search & Live Filters:** Search across account names, persona names, SteamID64s, and custom tags in real-time.
* **🎮 Fast Game Launching (AppLaunch):** Specify a game AppID (e.g. `730` for CS2, `570` for Dota 2) on any account; switching will automatically launch the game immediately!
* **📺 Advanced Steam Launch Flags:**
  * Toggle **Big Picture** mode (`-bigpicture`)
  * Toggle **Silent background start** (`-silent`)
  * Pass custom parameters (such as `-tcp`, `-dev`, `-novid`).
* **🛑 Steam Power Tools:**
  * **Force Kill Steam:** Terminate hung or unresponsive Steam processes.
  * **Restart Steam:** Gracefully restart the Steam client.
  * **Add New Account:** Automatically clears active login state to open the Steam login dialog cleanly.
  * **Open Folders:** 1-click access to the Steam directory and `userdata` (screenshots, configs, local save files).
  * **Server Status:** Instant shortcut to [Steamstat.us](https://steamstat.us/).
* **📌 System Tray (Minimize to Tray):** Runs quietly in your notification area with right-click quick controls.
* **📦 100% Standalone & Portable (.exe):** Single executable file with all runtimes bundled. No installer, no dependencies, no admin rights required.

---

### 🛡️ Safety, Security & Zero-Password Policy

1. **Zero Credentials Stored:** This app **never** asks for, reads, or transmits your passwords or Steam Guard codes.
2. **Native Steam Mechanism:** It relies entirely on Steam's official `"Remember my password"` feature (`config/loginusers.vdf`) and sets the `AutoLoginUser` registry key in `HKCU\Software\Valve\Steam`.
3. **No External Data Exfiltration:** The application only communicates with official Steam Web API endpoints (`api.steampowered.com`) if you choose to provide your personal Steam Web API key to retrieve avatar images and VAC ban status. All personal notes and tags stay 100% on your local disk in `%APPDATA%\SteamAccountSwitcherPro\`.

---

### 📥 Download & Usage

1. Download **`SteamAccountSwitcher.exe`** directly from [Releases](https://github.com/terdemm/steamaccountswitcher/releases/tag/v2.0.0).
2. Run `SteamAccountSwitcher.exe` anywhere (Desktop, USB, or Documents).
3. All accounts remembered by Steam will be automatically listed.
4. Click **"🚀 GEÇİŞ YAP" (SWITCH)** to switch to any account!

---

### 🔨 Building from Source

Requirements: .NET 8.0 SDK or Visual Studio 2022.

```powershell
# Clone the repository
git clone https://github.com/terdemm/steamaccountswitcher.git
cd steamaccountswitcher

# Publish single-file standalone portable binary
dotnet publish SteamAccountSwitcher.Launcher/SteamAccountSwitcher.Launcher.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o ./publish
```

---

<br/>

# 🇹🇷 Türkçe Dokümantasyon

### ✨ Öne Çıkan Özellikler

* **⚡ Tek Tıkla Anında Hesap Değiştirme:** İstediğiniz hesaba tıklayın; Steam arka planda güvenle kapatılır, seçilen hesap aktif edilir ve Steam saniyeler içinde yeni hesapla açılır.
* **🎨 Modern Karanlık Tasarım (Dark UI):** Windows 10 ve Windows 11 için optimize edilmiş, DirectX donanım hızlandırmalı, cam efektli ve modern rozetlere sahip arayüz.
* **🏷️ Kişisel Notlar ve Renkli Etiketler:** Hesaplarınıza *"Ana Hesap"*, *"CS2 Smurf"*, *"Pazar / Trade"*, *"Kardeşimin"* gibi açıklamalar ve renkli etiketler ekleyin.
* **⭐ Favori Hesaplar (Yıldızlama):** Sık kullandığınız hesapları yıldızlayarak her zaman en üstte tutun.
* **🔍 Canlı Arama ve Sekmeli Filtreleme:** Hesap adına, kullanıcı adına, SteamID'ye veya aldığınız nota göre anlık arama yapın (*Tüm Hesaplar*, *Favoriler*, *Aktif Oturum*).
* **🎮 Hızlı Oyun Başlatma (Fast AppLaunch):** Hesaba özel bir oyun AppID'si (örn. CS2 için `730`) belirleyin; hesaba geçtiğiniz anda oyun otomatik açılsın!
* **📺 Gelişmiş Başlatma Seçenekleri:**
  * Steam'i doğrudan **Big Picture** modunda açma.
  * Steam'i sessiz / arka planda başlatma (`-silent`).
  * Özel başlatma parametreleri (`-tcp`, `-novid` vb.).
* **⚡ Steam Hızlı Güç Araçları:**
  * **🛑 Steam'i Kapat:** Yanıt vermeyen Steam süreçlerini tek tıkla sonlandırın.
  * **🔄 Yeniden Başlat:** Steam'i temizce kapatıp yeniden başlatın.
  * **➕ Yeni Hesap Ekle:** Oturumu sıfırlayarak Steam login penceresini getirir.
  * **📁 Klasörleri Aç:** Steam dizinine ve `userdata` (ekran görüntüleri, yerel oyun kayıtları) klasörüne tek tıkla erişin.
  * **🌐 Sunucu Durumu:** [Steamstat.us](https://steamstat.us/) ile sunucu durumunu tek tıkla kontrol edin.
* **📌 Sistem Tepsisi (Tray Icon):** Simge durumuna küçültüldüğünde saatin yanına geçer, sağ tık menüsünden hızlı eylemler sunar.
* **📦 Tek Dosya Taşınabilir Exe (Portable):** Kurulum gerektirmez, DLL karmaşası yoktur, tek bir `.exe` dosyasından çalışır.

---

### 🛡️ Güvenlik & Gizlilik İlkeleri

* **Şifre İstemez ve Saklamaz:** Uygulama kullanıcı adı ve şifrelerinizi asla talep etmez veya kaydetmez.
* **Steam'in Resmi Mekanizmasını Kullanır:** Steam'de daha önce *"Beni Hatırla"* diyerek girdiğiniz oturumları (`loginusers.vdf`) ve Windows Kayıt Defterindeki (`AutoLoginUser`) anahtarını kullanır.
* **Veri Kaçırma Yoktur:** Kodlar tamamen şeffaf ve açık kaynaklıdır; hiçbir harici sunucuya veri göndermez. Notlarınız ve etiketleriniz sadece kendi bilgisayarınızda saklanır.

---

### 🛠️ Nasıl Kullanılır?

1. **[Releases Bölümünden](https://github.com/terdemm/steamaccountswitcher/releases/tag/v2.0.0)** **`SteamAccountSwitcher.exe`** dosyasını indirin ve çift tıklayarak çalıştırın.
2. Bilgisayarınızda kayıtlı tüm Steam hesapları otomatik olarak listelenecektir.
3. Geçmek istediğiniz hesabın yanındaki **"🚀 GEÇİŞ YAP"** butonuna basmanız yeterlidir.
4. Yeni bir hesap eklemek isterseniz üst bardaki **"➕ Hesap Ekle"** butonuna basın; açılan Steam giriş ekranında *"Beni Hatırla"* kutucuğunu işaretleyerek oturum açın.
5. Not eklemek için kart üzerindeki **"📝 Not / Ayar"** butonunu kullanabilirsiniz.

---

## 📜 Teşekkür & Referans

Bu proje, orijinal [sahin-a/SteamAccountSwitcher](https://github.com/sahin-a/SteamAccountSwitcher) projesinin temel araştırma ve mantığından esinlenilerek geliştirilmiş, arayüzü ve özellikleri modern gereksinimlere göre baştan yaratılmıştır. **sahin-a**'ya açık kaynağa sağladığı katkılardan dolayı teşekkür ederiz.

---

## 📄 Lisans
Bu proje [MIT Lisansı](LICENSE.md) kapsamında dağıtılmaktadır.
