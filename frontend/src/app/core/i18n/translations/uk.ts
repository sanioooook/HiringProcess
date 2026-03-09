export const uk: Record<string, string> = {
  // App shell
  'app.title': 'Трекер заявок на роботу',
  'app.theme.toDark': 'Темна тема',
  'app.theme.toLight': 'Світла тема',
  'app.menu.settings': 'Налаштування',
  'app.menu.signOut': 'Вийти',

  // Auth - login
  'auth.login.title': 'Вхід',
  'auth.login.subtitle': 'Трекер заявок на роботу',
  'auth.login.submit': 'Увійти',
  'auth.login.noAccount': 'Немає облікового запису?',
  'auth.login.register': 'Зареєструватися',
  'auth.login.forgotPassword': 'Забули пароль?',
  'auth.login.emailNotVerified': 'Будь ласка, підтвердьте email перед входом.',
  'auth.login.resendVerification': 'Надіслати лист підтвердження',
  'auth.login.verificationSent': 'Лист надіслано. Перевірте пошту.',

  // Auth - register
  'auth.register.title': 'Створити обліковий запис',
  'auth.register.subtitle': 'Трекер заявок на роботу',
  'auth.register.submit': 'Зареєструватися',
  'auth.register.hasAccount': 'Вже є обліковий запис?',
  'auth.register.signIn': 'Увійти',
  'auth.register.checkEmail': 'Обліковий запис створено! Перевірте пошту для підтвердження.',

  // Auth - verify email
  'auth.verifyEmail.verifying': 'Підтвердження email...',
  'auth.verifyEmail.success': 'Email підтверджено! Тепер можна увійти.',
  'auth.verifyEmail.error': 'Посилання недійсне або термін дії минув.',
  'auth.verifyEmail.signIn': 'Увійти',

  // Auth - forgot password
  'auth.forgotPassword.title': 'Відновлення пароля',
  'auth.forgotPassword.subtitle': 'Введіть email для отримання посилання на скидання пароля.',
  'auth.forgotPassword.submit': 'Надіслати посилання',
  'auth.forgotPassword.sent': 'Якщо обліковий запис існує, посилання надіслано.',
  'auth.forgotPassword.backToLogin': 'Назад до входу',

  // Auth - reset password
  'auth.resetPassword.title': 'Новий пароль',
  'auth.resetPassword.newPassword': 'Новий пароль',
  'auth.resetPassword.submit': 'Встановити пароль',
  'auth.resetPassword.success': 'Пароль оновлено! Тепер можна увійти.',
  'auth.resetPassword.error': 'Посилання недійсне або термін дії минув.',

  // Auth - confirm email change
  'auth.confirmEmailChange.confirming': 'Підтвердження нового email...',
  'auth.confirmEmailChange.success': 'Адресу email успішно змінено.',
  'auth.confirmEmailChange.error': 'Посилання недійсне або термін дії минув.',
  'auth.confirmEmailChange.signIn': 'Увійти',

  // Settings - change password
  'settings.changePassword.title': 'Змінити пароль',
  'settings.changePassword.current': 'Поточний пароль',
  'settings.changePassword.new': 'Новий пароль',
  'settings.changePassword.confirm': 'Підтвердіть новий пароль',
  'settings.changePassword.submit': 'Змінити пароль',
  'settings.changePassword.success': 'Пароль успішно змінено.',

  // Settings - change email
  'settings.changeEmail.title': 'Змінити email',
  'settings.changeEmail.current': 'Поточний email',
  'settings.changeEmail.new': 'Новий email',
  'settings.changeEmail.password': 'Поточний пароль',
  'settings.changeEmail.submit': 'Надіслати підтвердження',
  'settings.changeEmail.success': 'Підтвердження надіслано на нову адресу.',

  // Shared field labels
  'field.email': 'Email',
  'field.password': 'Пароль',
  'field.displayName': "Ім'я",
  'field.confirmPassword': 'Підтвердіть пароль',
  'field.companyName': 'Назва компанії',
  'field.contactChannel': 'Канал зв\'язку',
  'field.customChannel': 'Інший канал (якщо немає в списку)',
  'field.customChannelPlaceholder': 'напр. Telegram',
  'field.contactPerson': 'Контактна особа',
  'field.salaryRange': 'Діапазон зарплати',
  'field.salaryRangePlaceholder': 'напр. $80k–$100k',
  'field.firstContactDate': 'Дата першого контакту',
  'field.lastContactDate': 'Дата останнього контакту',
  'field.vacancyPublished': 'Дата публікації вакансії',
  'field.applicationDate': 'Дата подачі заявки',
  'field.hiringStages': 'Етапи найму',
  'field.addStagePreset': 'Додати етап (з шаблону)',
  'field.customStage': 'Власний етап',
  'field.customStagePlaceholder': 'Введіть і натисніть Enter',
  'field.currentStage': 'Поточний етап',
  'field.stageNone': '- Немає -',
  'field.appliedWith': 'Подано з',
  'field.appliedWithNone': '- Немає -',
  'field.appliedLink': 'Посилання на відгук',
  'field.vacancyLink': 'Посилання на вакансію',
  'field.coverLetter': 'Супровідний лист (підтримується Markdown)',
  'field.coverLetterPlaceholder': 'Напишіть ваш супровідний лист тут...',
  'field.vacancyText': 'Текст вакансії',
  'field.vacancyTextPlaceholder': 'Вставте опис вакансії сюди...',
  'field.notes': 'Нотатки (підтримується Markdown)',
  'field.notesPlaceholder': 'Ваші особисті нотатки...',

  // Client-side validation errors
  'validation.emailRequired': 'Email є обов\'язковим',
  'validation.emailInvalid': 'Введіть коректну адресу email',
  'validation.passwordRequired': 'Пароль є обов\'язковим',
  'validation.passwordMinLength': 'Мінімум 8 символів',
  'validation.passwordPattern': 'Має містити велику літеру та цифру',
  'validation.displayNameRequired': 'Ім\'я є обов\'язковим',
  'validation.passwordsMismatch': 'Паролі не збігаються',
  'validation.companyNameRequired': 'Назва компанії є обов\'язковою',
  'validation.contactChannelRequired': 'Канал зв\'язку є обов\'язковим',
  'validation.urlInvalid': 'Має бути коректний URL (https://...)',

  // List toolbar & table
  'list.search': 'Пошук',
  'list.searchPlaceholder': 'Компанія, етап, контакт...',
  'list.columns': 'Стовпці',
  'list.addApplication': 'Додати заявку',
  'list.noApplications': 'Заявок не знайдено.',
  'list.addFirst': 'Додати першу заявку',

  // Table column headers
  'col.company': 'Компанія',
  'col.stage': 'Етап',
  'col.channel': 'Канал',
  'col.contactPerson': 'Контактна особа',
  'col.applied': 'Подано',
  'col.firstContact': 'Перший контакт',
  'col.lastContact': 'Останній контакт',
  'col.salary': 'Зарплата',
  'col.appliedWith': 'Подано з',
  'col.stages': 'Етапи',
  'col.updated': 'Оновлено',
  'col.actions': 'Дії',

  // Row actions
  'action.edit': 'Редагувати',
  'action.delete': 'Видалити',
  'action.downloadFile': 'Завантажити файл',

  // Form dialog
  'dialog.addTitle': 'Додати заявку',
  'dialog.editTitle': 'Редагувати заявку',
  'dialog.cancel': 'Скасувати',
  'dialog.create': 'Створити',
  'dialog.saveChanges': 'Зберегти зміни',
  'dialog.tab.basicInfo': 'Основне',
  'dialog.tab.dates': 'Дати',
  'dialog.tab.pipeline': 'Етапи',
  'dialog.tab.notes': 'Нотатки',
  'dialog.tab.file': 'Файл вакансії',
  'dialog.addStage': 'Додати етап',

  // File tab
  'file.hasAttached': 'Файл вже прикріплено.',
  'file.download': 'Завантажити',
  'file.uploadInfo': 'Завантажте файл вакансії (PDF або TXT, до 10 МБ)',
  'file.chooseFile': 'Вибрати файл',

  // Column selector
  'colSelector.title': 'Керування стовпцями',
  'colSelector.cancel': 'Скасувати',
  'colSelector.apply': 'Застосувати',

  // Confirm dialog
  'confirm.deleteTitle': 'Видалити заявку',
  'confirm.deleteMessage': 'Видалити "{name}"? Цю дію не можна скасувати.',
  'confirm.deleteLabel': 'Видалити',

  // Settings
  'settings.title': 'Налаштування',
  'settings.language': 'Мова інтерфейсу',
  'settings.langEn': 'English',
  'settings.langUk': 'Українська',
  'settings.langRu': 'Русский',
  'settings.save': 'Зберегти',
  'settings.back': 'Назад',

  // Stage presets
  'stage.applied': 'Відгукнувся',
  'stage.phoneScreen': 'Телефонний скринінг',
  'stage.technicalInterview': 'Технічне інтерв\'ю',
  'stage.takeHomeTask': 'Тестове завдання',
  'stage.onsiteInterview': 'Очне інтерв\'ю',
  'stage.offerReceived': 'Отримано оффер',
  'stage.offerAccepted': 'Прийнято оффер',
  'stage.rejected': 'Відмова',
  'stage.withdrawn': 'Знято',

  // Snackbar messages
  'snack.deleted': 'Запис видалено.',
  'snack.deleteFailed': 'Помилка при видаленні.',
  'snack.loadFailed': 'Не вдалося завантажити записи.',
  'snack.savedFileUploadFailed': 'Запис збережено, але файл не завантажено.',
  'snack.saveFailed': 'Помилка збереження. Спробуйте ще раз.',
  'snack.settingsSaved': 'Мову збережено.',
  'snack.settingsFailed': 'Не вдалося зберегти налаштування.',
  'snack.sessionExpired': 'Сесія закінчилася. Будь ласка, увійдіть знову.',

  // Markdown toolbar
  'md.bold': 'Жирний',
  'md.italic': 'Курсив',
  'md.strikethrough': 'Закреслений',
  'md.heading': 'Заголовок',
  'md.list': 'Список',
  'md.quote': 'Цитата',
  'md.link': 'Посилання',
  'md.edit': 'Редагувати',
  'md.preview': 'Перегляд',
};
