# PC Usage Tracker

电脑使用时间统计工具，帮助你了解每日电脑使用习惯，提升时间管理效率。

## ✨ 功能特性

- **实时追踪** — 自动检测用户空闲状态，精确记录活跃/空闲时间
- **仪表盘** — 今日活跃时间、空闲时间、效率、会话数一目了然（10秒自动刷新）
- **周统计** — 柱状图展示本周每日使用时间分布（30秒自动刷新）
- **历史记录** — 浏览过去30天的每日使用详情
- **CSV 导出** — 一键导出周统计数据为 CSV 文件
- **系统托盘** — 关闭窗口后最小化到托盘，后台持续追踪
- **开机自启** — 支持设置系统启动时自动运行
- **空闲阈值** — 可自定义空闲检测时间（1-30分钟）

## 🖼 界面预览

- 暗色主题 UI，侧边导航栏
- 无边框窗口 + 自定义标题栏
- 卡片式数据展示
- 7日柱状图可视化

## 🛠 技术栈

| 层面 | 技术 |
|------|------|
| **语言** | VB.NET |
| **框架** | .NET 8 + WPF |
| **架构** | MVVM (CommunityToolkit.Mvvm) |
| **依赖注入** | Microsoft.Extensions.DependencyInjection |
| **数据库** | SQLite + Entity Framework Core |
| **系统交互** | Win32 API (P/Invoke) + WinForms 托盘 |

## 📁 项目结构

```
PcUsageTracker/
├── src/
│   ├── PcUsageTracker.Core/              # 业务逻辑层
│   │   ├── Enums/                        # 枚举定义
│   │   ├── Interfaces/                   # 接口定义
│   │   ├── Models/                       # 数据模型
│   │   └── Services/                     # 核心服务
│   │
│   ├── PcUsageTracker.Infrastructure/    # 基础设施层
│   │   ├── Data/                         # 数据库上下文 + 仓储
│   │   ├── DI/                           # 依赖注入配置
│   │   └── System/                       # 系统监控 (Win32 API)
│   │
│   └── PcUsageTracker.App/              # 表示层 (WPF UI)
│       ├── Converters/                   # 值转换器
│       ├── Resources/Styles/             # 样式资源
│       ├── ViewModels/                   # MVVM 视图模型
│       └── Views/                        # WPF 视图
│
├── PcUsageTracker.sln
└── .gitignore
```

## 🚀 快速开始

### 环境要求

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10/11

### 运行

```bash
cd src/PcUsageTracker.App
dotnet run
```

### 构建

```bash
dotnet build
```

构建产物位于 `src/PcUsageTracker.App/bin/Debug/net8.0-windows/`。

## 📦 依赖包

- **CommunityToolkit.Mvvm** — MVVM 工具包
- **Microsoft.Extensions.DependencyInjection** — 依赖注入
- **Microsoft.EntityFrameworkCore.Sqlite** — SQLite 数据库
- **LiveChartsCore.SkiaSharpView.WPF** — 图表库（预留）

## 📊 数据存储

使用 SQLite 数据库，数据文件位于 `%LocalAppData%\PcUsageTracker\` 目录下：

- `usage.db` — 使用会话和每日统计
- `config.txt` — 用户设置

## 📝 License

MIT License