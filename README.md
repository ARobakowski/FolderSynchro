# 🗂️ FolderSync – Folder Synchronization Tool

A simple one-way folder synchronization tool written in C#. It continuously syncs files from a **source folder** to a **replica folder** at a specified time interval. Any changes (added, modified, or deleted files) are mirrored in the replica folder and logged.

---

## 🛠 Features

- One-way sync from source → replica
- File comparison using **MD5 hash**
- Handles:
  - New files
  - Modified files
  - Deleted files
- Console and file logging
- Customizable sync interval (in seconds)
- Synchronization summary: number of files, duration, errors
- Asynchronous file operations handling
- Excluding files and directories based on patterns (e.g., .tmp, .git)

---

## 🚀 How to Run

### ✅ Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version 6 or newer)

### 🧪 Example Usage
```bash
dotnet run -- "<source_path>" "<replica_path>" <interval_seconds> "<log_file_path>"
```
## ▶️ Watch the demo
[![Watch the demo](https://img.youtube.com/vi/FzzqdCsoHBE/0.jpg)](https://www.youtube.com/watch?v=FzzqdCsoHBE)
