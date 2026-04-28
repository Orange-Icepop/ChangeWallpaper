# ChangeWallpaper

## 此项目已废弃 This project is obsolete

由于任何一个版本的 .NET 运行时的笨重对于这么一个小程序都是不可接受的，此项目即日起停止维护。

[基于Rust的新项目](https://github.com/Orange-Icepop/schools_wallpaper)已经发布，旨在通过AOT提供更好的性能与启动速度。

---

为了给学校的电脑按照当前星期几设置带有课程表的壁纸而写的一个命令行C#程序。由于是为了中国中学常用的Windows 7系统设计的，所以采用的是较老的 .NET 6。
## 使用方法
1、确保当前账户对程序运行目录有完全控制权。
2、解压release，启动程序，报错后退出。
3、打开config.json，修改配置。其中，键名为星期几的内容中填写目标图片的路径（由于在Windows上使用，请将作为Windows路径分隔符的反斜杠改为双反斜杠）。
4、再次运行程序，成功后程序会自动退出。
## 关于exception_setting
config.json文件中，键"exception_setting"的值代表是否使用临时配置。如果为true，则使用"exception"键中配置的图片，否则使用默认配置。