---
name: گزارش خطا
description: گزارش یک خطا در نرم‌افزار
about: درباره این الگو
title: "[خطا]: "
labels: ["bug", "triage"]
projects: ["نام-سازمان/1", "نام-سازمان/44"]
assignees:
  - نام-کاربر
body:
  - type: markdown
    attributes:
      value: |
        با تشکر از شما که وقت گذاشته‌اید تا این گزارش خطا را پر کنید!
  - type: input
    id: contact
    attributes:
      label: جزئیات تماس
      description: چگونه می‌توانیم با شما تماس بگیریم اگر نیاز به اطلاعات بیشتر داریم؟
      placeholder: مثلاً email@example.com
      validations:
        required: false
  - type: textarea
    id: what-happened
    attributes:
      label: چه اتفاقی افتاد؟
      description: همچنین به ما بگوید، چه چیزی انتظار داشتید که اتفاق بیفتد؟
      placeholder: به ما بگوید چه چیزی مشاهده کردید!
      value: "یک خطا رخ داد!"
---
