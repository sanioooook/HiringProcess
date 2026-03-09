namespace HiringProcess.Api.Common.Localization;

public sealed class LocalizationService : ILocalizationService
{
    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["en"] = new()
        {
            // Auth — field validation
            ["auth.emailRequired"] = "Email is required.",
            ["auth.emailInvalid"] = "Email must be a valid email address.",
            ["auth.displayNameRequired"] = "Display name is required.",
            ["auth.passwordRequired"] = "Password is required.",
            ["auth.passwordMinLength"] = "Password must be at least 8 characters.",
            ["auth.passwordUppercase"] = "Password must contain at least one uppercase letter.",
            ["auth.passwordDigit"] = "Password must contain at least one digit.",
            // Auth — business errors
            ["auth.invalidCredentials"] = "Invalid email or password.",
            ["auth.emailTaken"] = "An account with this email already exists.",
            ["auth.googleTokenRequired"] = "Google ID token is required.",
            ["auth.googleFailed"] = "Google authentication failed. Please try again.",
            ["auth.emailNotVerified"] = "Please verify your email address before signing in.",
            ["auth.tokenRequired"] = "Token is required.",
            ["auth.tokenInvalidOrExpired"] = "The link is invalid or has expired.",
            ["auth.noPasswordSet"] = "This account uses Google sign-in and has no password.",
            // Email templates
            ["email.verifySubject"] = "Verify your email address",
            ["email.verifyBody"] = "Click the link below to verify your email address:",
            ["email.resetSubject"] = "Reset your password",
            ["email.resetBody"] = "Click the link below to reset your password. The link expires in 1 hour:",
            ["email.changeEmailSubject"] = "Confirm your new email address",
            ["email.changeEmailBody"] = "Click the link below to confirm your new email address:",
            // Hiring processes — field validation
            ["hp.companyRequired"] = "Company name is required.",
            ["hp.channelRequired"] = "Contact channel is required.",
            ["hp.appliedLinkUrl"] = "Applied link must be a valid URL.",
            ["hp.vacancyLinkUrl"] = "Vacancy link must be a valid URL.",
            ["hp.stageTooLong"] = "Each hiring stage must be at most 200 characters.",
            ["hp.idRequired"] = "Id is required.",
            // Hiring processes — business errors
            ["hp.notFound"] = "Hiring process not found.",
            ["hp.noFile"] = "This hiring process has no attached vacancy file.",
            ["hp.fileNotFound"] = "The vacancy file could not be found in storage.",
            ["hp.onlyPdfTxt"] = "Only PDF and TXT files are accepted.",
            ["hp.pageSize"] = "Page must be >= 1 and PageSize must be between 1 and 100.",
            // User settings
            ["settings.invalidLanguage"] = "Language must be one of: en, uk, ru.",
            ["settings.userNotFound"] = "User not found.",
        },
        ["uk"] = new()
        {
            // Auth — field validation
            ["auth.emailRequired"] = "Email є обов'язковим.",
            ["auth.emailInvalid"] = "Введіть коректну адресу email.",
            ["auth.displayNameRequired"] = "Ім'я обов'язкове.",
            ["auth.passwordRequired"] = "Пароль є обов'язковим.",
            ["auth.passwordMinLength"] = "Пароль має містити не менше 8 символів.",
            ["auth.passwordUppercase"] = "Пароль має містити хоча б одну велику літеру.",
            ["auth.passwordDigit"] = "Пароль має містити хоча б одну цифру.",
            // Auth — business errors
            ["auth.invalidCredentials"] = "Невірний email або пароль.",
            ["auth.emailTaken"] = "Обліковий запис з таким email вже існує.",
            ["auth.googleTokenRequired"] = "Google ID токен є обов'язковим.",
            ["auth.googleFailed"] = "Помилка автентифікації через Google. Спробуйте ще раз.",
            ["auth.emailNotVerified"] = "Будь ласка, підтвердьте свою адресу email перед входом.",
            ["auth.tokenRequired"] = "Токен є обов'язковим.",
            ["auth.tokenInvalidOrExpired"] = "Посилання недійсне або термін його дії минув.",
            ["auth.noPasswordSet"] = "Цей обліковий запис використовує вхід через Google і не має пароля.",
            // Email templates
            ["email.verifySubject"] = "Підтвердьте вашу адресу email",
            ["email.verifyBody"] = "Натисніть посилання нижче, щоб підтвердити вашу адресу email:",
            ["email.resetSubject"] = "Скидання пароля",
            ["email.resetBody"] = "Натисніть посилання нижче, щоб скинути пароль. Посилання дійсне протягом 1 години:",
            ["email.changeEmailSubject"] = "Підтвердьте нову адресу email",
            ["email.changeEmailBody"] = "Натисніть посилання нижче, щоб підтвердити нову адресу email:",
            // Hiring processes — field validation
            ["hp.companyRequired"] = "Назва компанії є обов'язковою.",
            ["hp.channelRequired"] = "Канал зв'язку є обов'язковим.",
            ["hp.appliedLinkUrl"] = "Посилання на відгук має бути коректним URL.",
            ["hp.vacancyLinkUrl"] = "Посилання на вакансію має бути коректним URL.",
            ["hp.stageTooLong"] = "Кожен етап найму не може перевищувати 200 символів.",
            ["hp.idRequired"] = "Id є обов'язковим.",
            // Hiring processes — business errors
            ["hp.notFound"] = "Процес найму не знайдено.",
            ["hp.noFile"] = "До цього процесу найму не прикріплено жодного файлу.",
            ["hp.fileNotFound"] = "Файл вакансії не знайдено у сховищі.",
            ["hp.onlyPdfTxt"] = "Дозволені лише файли PDF та TXT.",
            ["hp.pageSize"] = "Сторінка має бути >= 1, а розмір сторінки — від 1 до 100.",
            // User settings
            ["settings.invalidLanguage"] = "Мова має бути однією з: en, uk, ru.",
            ["settings.userNotFound"] = "Користувача не знайдено.",
        },
        ["ru"] = new()
        {
            // Auth — field validation
            ["auth.emailRequired"] = "Email обязателен.",
            ["auth.emailInvalid"] = "Введите корректный адрес email.",
            ["auth.displayNameRequired"] = "Имя обязательно.",
            ["auth.passwordRequired"] = "Пароль обязателен.",
            ["auth.passwordMinLength"] = "Пароль должен содержать не менее 8 символов.",
            ["auth.passwordUppercase"] = "Пароль должен содержать хотя бы одну заглавную букву.",
            ["auth.passwordDigit"] = "Пароль должен содержать хотя бы одну цифру.",
            // Auth — business errors
            ["auth.invalidCredentials"] = "Неверный email или пароль.",
            ["auth.emailTaken"] = "Аккаунт с таким email уже существует.",
            ["auth.googleTokenRequired"] = "Google ID токен обязателен.",
            ["auth.googleFailed"] = "Ошибка аутентификации через Google. Попробуйте ещё раз.",
            ["auth.emailNotVerified"] = "Пожалуйста, подтвердите адрес email перед входом.",
            ["auth.tokenRequired"] = "Токен обязателен.",
            ["auth.tokenInvalidOrExpired"] = "Ссылка недействительна или срок её действия истёк.",
            ["auth.noPasswordSet"] = "Этот аккаунт использует вход через Google и не имеет пароля.",
            // Email templates
            ["email.verifySubject"] = "Подтвердите адрес email",
            ["email.verifyBody"] = "Нажмите ссылку ниже, чтобы подтвердить адрес email:",
            ["email.resetSubject"] = "Сброс пароля",
            ["email.resetBody"] = "Нажмите ссылку ниже, чтобы сбросить пароль. Ссылка действительна 1 час:",
            ["email.changeEmailSubject"] = "Подтвердите новый адрес email",
            ["email.changeEmailBody"] = "Нажмите ссылку ниже, чтобы подтвердить новый адрес email:",
            // Hiring processes — field validation
            ["hp.companyRequired"] = "Название компании обязательно.",
            ["hp.channelRequired"] = "Канал связи обязателен.",
            ["hp.appliedLinkUrl"] = "Ссылка на отклик должна быть корректным URL.",
            ["hp.vacancyLinkUrl"] = "Ссылка на вакансию должна быть корректным URL.",
            ["hp.stageTooLong"] = "Каждый этап найма не может превышать 200 символов.",
            ["hp.idRequired"] = "Id обязателен.",
            // Hiring processes — business errors
            ["hp.notFound"] = "Процесс найма не найден.",
            ["hp.noFile"] = "К этому процессу найма не прикреплён файл.",
            ["hp.fileNotFound"] = "Файл вакансии не найден в хранилище.",
            ["hp.onlyPdfTxt"] = "Разрешены только файлы PDF и TXT.",
            ["hp.pageSize"] = "Страница должна быть >= 1, а размер страницы — от 1 до 100.",
            // User settings
            ["settings.invalidLanguage"] = "Язык должен быть одним из: en, uk, ru.",
            ["settings.userNotFound"] = "Пользователь не найден.",
        },
    };

    public string Get(string key, string language)
    {
        var lang = _translations.ContainsKey(language) ? language : "en";
        return _translations[lang].TryGetValue(key, out var msg) ? msg : key;
    }
}
