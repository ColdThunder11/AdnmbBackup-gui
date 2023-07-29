# AdnmbBackup-gui

一个让你本地备份A岛串的小程序。

已完美适配X岛

备份需要获取饼干，这里是[饼干获取教程](https://www.coldthunder11.com/index.php/2020/03/19/%e5%a6%82%e4%bd%95%e8%8e%b7%e5%8f%96a%e5%b2%9b%e7%9a%84%e9%a5%bc%e5%b9%b2/)

支持批量自动备份，方法如下：

1. 在程序目录下新建`uuid.txt`，写入移动端订阅id，每次打开会自动将所有订阅的串号加入自动备份列表
2. 在程序目录下新建`AutoBackupList.txt`，可以手动加入其他需要备份的串号填入（一行一个）

每天第一次打开会自动备份所有在`AutoBackupList.txt`中的串，备份文件在`output`文件夹中。

需要 .net framework4.8。

json文件为完整数据文件，txt为易读的文件，md为markdown格式的文件包含图片及部分格式。

目前文件结构：

``` bash
├─AdnmbBackup-gui
│  ├─po
│  │  ├─*.txt // 内容将覆盖po的饼干。可多行，一行一个
|  ├─cache
|  |  ├─*.json // 原始数据文件，也即缓存文件
|  ├─output
|  |  ├─all
|  |  |  ├─*.txt // 串的所有内容的txt文件
|  |  |  ├─*.md // 串的所有内容的md文件
|  |  ├─po
|  |  |  ├─*_po_only.txt // po的内容的txt文件
|  |  |  ├─*_po_only.md // po的内容的md文件
|  |  ├─*.txt // 串的所有内容的txt文件
|  |  ├─*.md // 串的所有内容的md文件
|  |  ├─*_po_only.txt // po的内容的txt文件
|  |  ├─*_po_only.md // po的内容的md文件
|  ├─AdnmbBackup-gui.exe
│  ├─cookie.txt // 饼干
│  ├─Newtonsoft.Json.dll
│  ├─Newtonsoft.Json.xml
│  ├─AutoBackupList.txt // 自动备份列表,一行一个,填id
```

TODO:

- [x] 增量备份
- [ ] 串号从剪贴板获取
- [ ] 任务计划自动备份（或教程）
- [ ] Cookie获取教程
- [x] 方便的加入自动备份列表
- [ ] 方便的更改po
- [ ] 文件名系统
- [x] markdown格式备份支持
- [x] markdown中插入图片
- [x] 重命名文件
