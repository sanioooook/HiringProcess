export const ru: Record<string, string> = {
  // App shell
  'app.title': 'Трекер заявок на работу',
  'app.theme.toDark': 'Тёмная тема',
  'app.theme.toLight': 'Светлая тема',
  'app.menu.settings': 'Настройки',
  'app.menu.signOut': 'Выйти',

  // Auth — login
  'auth.login.title': 'Вход',
  'auth.login.subtitle': 'Трекер заявок на работу',
  'auth.login.submit': 'Войти',
  'auth.login.noAccount': 'Нет аккаунта?',
  'auth.login.register': 'Зарегистрироваться',

  // Auth — register
  'auth.register.title': 'Создать аккаунт',
  'auth.register.subtitle': 'Трекер заявок на работу',
  'auth.register.submit': 'Зарегистрироваться',
  'auth.register.hasAccount': 'Уже есть аккаунт?',
  'auth.register.signIn': 'Войти',

  // Shared field labels
  'field.email': 'Email',
  'field.password': 'Пароль',
  'field.displayName': 'Имя',
  'field.confirmPassword': 'Подтвердите пароль',
  'field.companyName': 'Название компании',
  'field.contactChannel': 'Канал связи',
  'field.customChannel': 'Другой канал (если нет в списке)',
  'field.customChannelPlaceholder': 'напр. Telegram',
  'field.contactPerson': 'Контактное лицо',
  'field.salaryRange': 'Диапазон зарплаты',
  'field.salaryRangePlaceholder': 'напр. $80k–$100k',
  'field.firstContactDate': 'Дата первого контакта',
  'field.lastContactDate': 'Дата последнего контакта',
  'field.vacancyPublished': 'Дата публикации вакансии',
  'field.applicationDate': 'Дата подачи заявки',
  'field.hiringStages': 'Этапы найма',
  'field.addStagePreset': 'Добавить этап (из шаблона)',
  'field.customStage': 'Свой этап',
  'field.customStagePlaceholder': 'Введите и нажмите Enter',
  'field.currentStage': 'Текущий этап',
  'field.stageNone': '— Нет —',
  'field.appliedWith': 'Подано с',
  'field.appliedWithNone': '— Нет —',
  'field.appliedLink': 'Ссылка на отклик',
  'field.vacancyLink': 'Ссылка на вакансию',
  'field.coverLetter': 'Сопроводительное письмо (поддерживается Markdown)',
  'field.coverLetterPlaceholder': 'Напишите ваше сопроводительное письмо здесь...',
  'field.vacancyText': 'Текст вакансии',
  'field.vacancyTextPlaceholder': 'Вставьте описание вакансии сюда...',
  'field.notes': 'Заметки (поддерживается Markdown)',
  'field.notesPlaceholder': 'Ваши личные заметки...',

  // Client-side validation errors
  'validation.emailRequired': 'Email обязателен',
  'validation.emailInvalid': 'Введите корректный адрес email',
  'validation.passwordRequired': 'Пароль обязателен',
  'validation.passwordMinLength': 'Минимум 8 символов',
  'validation.passwordPattern': 'Должен содержать заглавную букву и цифру',
  'validation.displayNameRequired': 'Имя обязательно',
  'validation.passwordsMismatch': 'Пароли не совпадают',
  'validation.companyNameRequired': 'Название компании обязательно',
  'validation.contactChannelRequired': 'Канал связи обязателен',
  'validation.urlInvalid': 'Должен быть корректный URL (https://...)',

  // List toolbar & table
  'list.search': 'Поиск',
  'list.searchPlaceholder': 'Компания, этап, контакт...',
  'list.columns': 'Столбцы',
  'list.addApplication': 'Добавить заявку',
  'list.noApplications': 'Заявок не найдено.',
  'list.addFirst': 'Добавить первую заявку',

  // Table column headers
  'col.company': 'Компания',
  'col.stage': 'Этап',
  'col.channel': 'Канал',
  'col.contactPerson': 'Контактное лицо',
  'col.applied': 'Подано',
  'col.firstContact': 'Первый контакт',
  'col.lastContact': 'Последний контакт',
  'col.salary': 'Зарплата',
  'col.appliedWith': 'Подано с',
  'col.stages': 'Этапы',
  'col.updated': 'Обновлено',
  'col.actions': 'Действия',

  // Row actions
  'action.edit': 'Редактировать',
  'action.delete': 'Удалить',
  'action.downloadFile': 'Скачать файл',

  // Form dialog
  'dialog.addTitle': 'Добавить заявку',
  'dialog.editTitle': 'Редактировать заявку',
  'dialog.cancel': 'Отмена',
  'dialog.create': 'Создать',
  'dialog.saveChanges': 'Сохранить изменения',
  'dialog.tab.basicInfo': 'Основное',
  'dialog.tab.dates': 'Даты',
  'dialog.tab.pipeline': 'Этапы',
  'dialog.tab.notes': 'Заметки',
  'dialog.tab.file': 'Файл вакансии',
  'dialog.addStage': 'Добавить этап',

  // File tab
  'file.hasAttached': 'Файл уже прикреплён.',
  'file.download': 'Скачать',
  'file.uploadInfo': 'Загрузите файл вакансии (PDF или TXT, до 10 МБ)',
  'file.chooseFile': 'Выбрать файл',

  // Column selector
  'colSelector.title': 'Управление столбцами',
  'colSelector.cancel': 'Отмена',
  'colSelector.apply': 'Применить',

  // Confirm dialog
  'confirm.deleteTitle': 'Удалить заявку',
  'confirm.deleteMessage': 'Удалить "{name}"? Это действие нельзя отменить.',
  'confirm.deleteLabel': 'Удалить',

  // Settings
  'settings.title': 'Настройки',
  'settings.language': 'Язык интерфейса',
  'settings.langEn': 'English',
  'settings.langUk': 'Українська',
  'settings.langRu': 'Русский',
  'settings.save': 'Сохранить',
  'settings.back': 'Назад',

  // Stage presets
  'stage.applied': 'Откликнулся',
  'stage.phoneScreen': 'Телефонный скрининг',
  'stage.technicalInterview': 'Техническое интервью',
  'stage.takeHomeTask': 'Тестовое задание',
  'stage.onsiteInterview': 'Очное интервью',
  'stage.offerReceived': 'Оффер получен',
  'stage.offerAccepted': 'Оффер принят',
  'stage.rejected': 'Отказ',
  'stage.withdrawn': 'Отозван',

  // Snackbar messages
  'snack.deleted': 'Запись удалена.',
  'snack.deleteFailed': 'Ошибка при удалении.',
  'snack.loadFailed': 'Не удалось загрузить записи.',
  'snack.savedFileUploadFailed': 'Запись сохранена, но файл не загружен.',
  'snack.saveFailed': 'Ошибка сохранения. Попробуйте ещё раз.',
  'snack.settingsSaved': 'Язык сохранён.',
  'snack.settingsFailed': 'Не удалось сохранить настройки.',
  'snack.sessionExpired': 'Сессия истекла. Пожалуйста, войдите снова.',

  // Markdown toolbar
  'md.bold': 'Жирный',
  'md.italic': 'Курсив',
  'md.strikethrough': 'Зачёркнутый',
  'md.heading': 'Заголовок',
  'md.list': 'Список',
  'md.quote': 'Цитата',
  'md.link': 'Ссылка',
  'md.edit': 'Редактировать',
  'md.preview': 'Просмотр',
};
