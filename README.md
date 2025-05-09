# Quizard

**Quizard** is a responsive web-based quiz application built with ASP.NET Core Razor Pages and Entity Framework Core. It allows users to take general knowledge quizzes, track their progress, and review past attempts—all in an engaging and user-friendly interface.

---

## 🚀 Features

- Multiple quizzes with configurable time limits
- Question editor with multi-choice and single-choice support
- Answer tracking with autosave and background submission
- Score calculation and progress history
- Resume or retake incomplete and completed attempts
- In-memory distributed cache for autosaving quiz answers
- Countdown timer with auto-submission on expiry
- Answer review with visual feedback on correctness
- Index pagination

---

## 🛠️ Tech Stack

- **Frontend:** Razor Pages + Bootstrap 5
- **Backend:** ASP.NET Core 8 with C#
- **Database:** Entity Framework Core (SQL Server or SQLite)
- **Caching:** IDistributedCache
- **Background Processing:** Hosted service

---

## 🧩 Future Enhancements
### 🔐 Authentication & Permissions
- [ ] Add user login system (ASP.NET Identity or external OAuth)
- [ ] Restrict quiz history per user

### 📊 Dashboard & Analytics
- [ ] Add quiz and question categories
- [ ] Add per-question feedback or explanations
- [ ] Show leaderboard or average stats

### 🔄 Import/Export
- [ ] Export quiz results to CSV or PDF
- [ ] Bulk import quizzes via JSON or Excel

### 🧪 Testing
- [ ] Add unit/integration tests for services and model validation
- [ ] Include test coverage reporting