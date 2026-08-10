# 📧 MailScope

### Email Intelligence & Contact Analyzer

**MailScope** 是一款基于 **C# / .NET / WPF / MailKit** 开发的桌面邮箱分析工具。

它通过 **IMAP 协议**读取邮箱中的邮件信息，在不下载邮件正文的情况下，快速分析收件箱与发件箱中的联系人，并统计：

* 📊 邮箱交流次数
* 🕐 最近联系时间
* 👥 联系人数量
* 📈 扫描进度与处理速度
* 📁 Excel 分析结果导出

> **MailScope | Email Intelligence & Contact Analyzer**

---

## ✨ Features

### 📬 邮箱扫描

目前支持通过 IMAP 连接邮箱服务器进行分析。

支持扫描：

* 📥 Inbox / 收件箱
* 📤 Sent / 发件箱

程序只读取邮件的 **Envelope / Header 信息**，不会下载邮件正文及附件，因此相比完整下载邮件具有更低的网络开销。

---

### 👤 联系人统计

MailScope 会自动提取邮件中的：

* From
* To
* CC

并根据邮箱地址进行去重统计。

例如：

```text
user@example.com       128
admin@example.com       86
test@example.com        42
```

同时记录每个联系人的：

| 字段   | 说明            |
| ---- | ------------- |
| 邮箱   | 联系人的 Email 地址 |
| 交流次数 | 在扫描邮件中出现的次数   |
| 最近联系 | 最近一次邮件时间      |

---

### 📊 实时扫描进度

扫描过程中实时显示：

```text
Total Mail
Finished Mail
Contact Count
Current Folder
Speed
```

例如：

```text
Inbox: 500 / 2706

扫描速度：12.35 mails/s
发现联系人：177
```

同时支持实时日志输出，方便观察 IMAP 连接及扫描状态。

---

### 🔄 IMAP 自动恢复

针对部分邮箱服务器存在的：

* IMAP 连接超时
* 网络异常
* Server unexpectedly disconnected
* Connection lost
* Authentication 状态失效

MailScope 提供了 IMAP 连接检测与自动重新连接机制。

核心逻辑包括：

```text
连接检测
   ↓
发现连接异常
   ↓
断开旧连接
   ↓
重新建立 IMAP 连接
   ↓
重新认证
   ↓
继续扫描
```

这样可以提高大量邮件扫描时的稳定性。

---

### 📦 批量读取

邮件采用批量方式读取，而不是一次性加载全部邮件。

默认：

```text
BatchSize = 100
```

每次读取一批邮件 Header / Envelope 信息。

这种方式能够降低：

* 内存占用
* IMAP 请求压力
* 单次请求数据量

---

### 📑 Excel 导出

分析完成后可以将联系人统计结果导出为 Excel。

默认结果包含：

| 邮箱                                            | 交流次数 | 最近联系             |
| --------------------------------------------- | ---: | ---------------- |
| [user@example.com](mailto:user@example.com)   |  128 | 2026-08-10 14:32 |
| [admin@example.com](mailto:admin@example.com) |   86 | 2026-08-09 18:21 |

方便后续进行：

* 联系人整理
* 客户分析
* 邮箱数据统计
* Excel 二次处理

---

## 🖥️ Screenshot

> Screenshots can be added here after the UI is finalized.

```text
┌──────────────────────────────────────────────────────────────┐
│ 📧 MailScope                                                  │
│ Email Intelligence & Contact Analyzer                        │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Email Account:  user@example.com                            │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │ Inbox: 1500 / 2706                                    │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
│  Contacts: 177        Speed: 12.35 mails/s                  │
│                                                              │
│  ─────────────────────────────────────────────────────────  │
│  IMAP连接成功                                                │
│  Inbox:100/2706                                              │
│  Inbox:200/2706                                              │
│  ...                                                         │
│                                                              │
├──────────────────────────────────────────────────────────────┤
│ MailScope | Email Intelligence & Contact Analyzer             │
│                                      Developed by Xgg         │
└──────────────────────────────────────────────────────────────┘
```

---

# 🛠️ Technology Stack

| Technology                | Purpose                   |
| ------------------------- | ------------------------- |
| **C#**                    | Main programming language |
| **.NET 10**               | Application framework     |
| **WPF**                   | Desktop UI                |
| **MailKit**               | IMAP email communication  |
| **MimeKit**               | Email / MIME parsing      |
| **EPPlus**                | Excel export              |
| **CommunityToolkit.Mvvm** | MVVM support              |

---

# 🏗️ Architecture

项目整体结构：

```text
MailScope
│
├── Models
│   ├── EmailContact.cs
│   ├── MailProvider.cs
│   └── ScanStatus.cs
│
├── Services
│   └── MailService.cs
│
├── ViewModels
│   └── MainViewModel.cs
│
├── Views
│   └── MainWindow.xaml
│
├── Resources
│
└── App.xaml
```

核心数据流程：

```text
        ┌──────────────┐
        │ Email Server │
        └──────┬───────┘
               │
              IMAP
               │
               ▼
        ┌──────────────┐
        │  MailService │
        └──────┬───────┘
               │
       Envelope / Header
               │
               ▼
        ┌──────────────┐
        │Contact Parser│
        └──────┬───────┘
               │
               ▼
       ┌─────────────────┐
       │ EmailContact    │
       │                 │
       │ Email           │
       │ Count           │
       │ LastTime        │
       └────────┬────────┘
                │
        ┌───────┴────────┐
        ▼                ▼
   WPF UI显示         Excel导出
```

---

# 🚀 Getting Started

## Requirements

建议开发环境：

```text
Windows 10 / Windows 11
.NET 10 SDK
Visual Studio 2022 / Visual Studio 2026
```

项目目标框架：

```xml
<TargetFramework>net10.0-windows</TargetFramework>
```

---

## 📥 Clone

```bash
git clone https://github.com/Xugg-1999/MailScope.git
```


恢复 NuGet 包：

```bash
dotnet restore
```

运行：

```bash
dotnet run
```

---

# 🔐 Email Configuration

MailScope 使用 IMAP 连接邮箱服务器。

需要提供：

```text
Email Account
Email Password
IMAP Server
IMAP Port
SSL/TLS
```

例如：

```text
Account:
user@example.com

IMAP Server:
imap.example.com

Port:
993

SSL:
Enabled
```

> 对于部分邮箱服务商，需要使用 **IMAP 专用密码 / 授权码**，而不是网页登录密码。

---

# 📖 How to Use

### 1. 输入邮箱账号

在邮箱账号输入框中填写需要分析的邮箱。

### 2. 输入密码 / 授权码

填写邮箱对应的 IMAP 密码或授权码。

### 3. 选择邮箱服务商

根据实际邮箱配置选择对应的 IMAP 服务。

### 4. 开始分析

点击：

```text
开始分析
```

程序将：

```text
连接 IMAP
    ↓
验证账号
    ↓
获取邮箱目录
    ↓
定位 Inbox / Sent
    ↓
批量读取邮件 Header
    ↓
提取 From / To / CC
    ↓
统计联系人
    ↓
更新扫描进度
    ↓
生成分析结果
```

### 5. 导出结果

扫描完成后，可以将联系人分析结果导出为 Excel。

---

# ⚡ Performance

MailScope 默认采用批量 Header 读取：

```text
BatchSize = 100
```

不会下载：

```text
✘ 邮件正文
✘ 附件
✘ 图片
✘ HTML Body
```

主要读取：

```text
✔ From
✔ To
✔ CC
✔ Date
✔ Envelope
```

因此对于拥有数千封甚至更多邮件的邮箱，可以明显降低扫描过程中的网络数据量。

---

# 🔄 Connection Recovery

长时间运行 IMAP 扫描时，服务器可能主动断开连接。

MailScope 会检查：

```csharp
_client.IsConnected
_client.IsAuthenticated
```

当发现连接不可用时：

```text
Disconnect
    ↓
Wait
    ↓
Connect
    ↓
Authenticate
    ↓
Continue Scan
```

从而避免因为单次网络连接异常导致整个扫描任务失败。

---

# 📊 Example

例如一个邮箱：

```text
Total Mail: 2769
Inbox: 2706
Sent: 63
```

扫描完成后：

```text
发现联系人: 177
```

最终按照交流次数排序：

```text
someone@example.com      523
admin@example.com        286
user@example.com         173
test@example.com          91
...
```

可以快速了解：

> 这个邮箱主要和哪些人进行过邮件交流，以及最近一次联系是什么时候。

---

# 🧩 Supported Email Providers

目前设计上支持通过 `MailProvider` 配置不同邮箱服务商。

例如：

```csharp
public class MailProvider
{
    public string ImapHost { get; set; }
    public int Port { get; set; }
    public bool UseSsl { get; set; }
}
```

因此后续可以扩展：

```text
Aliyun Mail
Microsoft 365
Gmail
QQ Mail
163 Mail
企业邮箱
自定义 IMAP Server
```

只要邮箱服务商提供标准 IMAP 服务即可。

---

# 🗺️ Roadmap

未来可以考虑增加：

* [ ] 多邮箱账号分析
* [ ] 联系人分类
* [ ] 收件 / 发件分别统计
* [ ] 联系频率趋势图
* [ ] 月度 / 年度邮件统计
* [ ] Top Contacts
* [ ] 邮箱域名统计
* [ ] 联系人搜索
* [ ] 邮件时间分布
* [ ] 联系人关系分析
* [ ] Excel 自定义导出
* [ ] CSV 导出
* [ ] 更多邮箱服务商
* [ ] 更完善的 IMAP 自动恢复
* [ ] 大规模邮箱扫描优化

---

# ⚠️ Disclaimer

MailScope 仅用于邮箱数据分析和个人生产力场景。

请确保：

* 你拥有所分析邮箱的合法使用权限
* 遵守邮箱服务商的服务条款
* 遵守所在地区的数据保护和隐私相关法律法规

MailScope 不会主动上传或共享你的邮件内容。

---

# 📄 License

This project is currently intended for personal and internal use.

License information may be added in future releases.

---

# 👨‍💻 Author

**Xgg**

MailScope is an independent project developed with:

```text
C#
.NET
WPF
MailKit
MimeKit
EPPlus
```

---

<div align="center">

### 📧 MailScope

**Email Intelligence & Contact Analyzer**

Powered by [Xugg](mailto:xugg1999@outlook.com)

</div>
