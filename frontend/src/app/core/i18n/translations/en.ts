export const en: Record<string, string> = {
  // App shell
  'app.title': 'Job Application Tracker',
  'app.theme.toDark': 'Switch to dark theme',
  'app.theme.toLight': 'Switch to light theme',
  'app.menu.settings': 'Settings',
  'app.menu.signOut': 'Sign Out',

  // Auth - login
  'auth.login.title': 'Sign In',
  'auth.login.subtitle': 'Job Application Tracker',
  'auth.login.submit': 'Sign In',
  'auth.login.noAccount': "Don't have an account?",
  'auth.login.register': 'Register',
  'auth.login.forgotPassword': 'Forgot password?',
  'auth.login.emailNotVerified': 'Please verify your email before signing in.',
  'auth.login.resendVerification': 'Resend verification email',
  'auth.login.verificationSent': 'Verification email sent. Check your inbox.',

  // Auth - register
  'auth.register.title': 'Create Account',
  'auth.register.subtitle': 'Job Application Tracker',
  'auth.register.submit': 'Create Account',
  'auth.register.hasAccount': 'Already have an account?',
  'auth.register.signIn': 'Sign In',
  'auth.register.checkEmail': 'Account created! Check your email to verify your address.',

  // Auth - verify email
  'auth.verifyEmail.verifying': 'Verifying your email...',
  'auth.verifyEmail.success': 'Email verified! You can now sign in.',
  'auth.verifyEmail.error': 'The link is invalid or has expired.',
  'auth.verifyEmail.signIn': 'Sign In',

  // Auth - forgot password
  'auth.forgotPassword.title': 'Forgot Password',
  'auth.forgotPassword.subtitle': 'Enter your email to receive a reset link.',
  'auth.forgotPassword.submit': 'Send Reset Link',
  'auth.forgotPassword.sent': 'If an account exists, a reset link has been sent.',
  'auth.forgotPassword.backToLogin': 'Back to Sign In',

  // Auth - reset password
  'auth.resetPassword.title': 'Reset Password',
  'auth.resetPassword.newPassword': 'New Password',
  'auth.resetPassword.submit': 'Set New Password',
  'auth.resetPassword.success': 'Password updated! You can now sign in.',
  'auth.resetPassword.error': 'The link is invalid or has expired.',

  // Auth - confirm email change
  'auth.confirmEmailChange.confirming': 'Confirming your new email...',
  'auth.confirmEmailChange.success': 'Email address updated successfully.',
  'auth.confirmEmailChange.error': 'The link is invalid or has expired.',
  'auth.confirmEmailChange.signIn': 'Sign In',

  // Settings - change password
  'settings.changePassword.title': 'Change Password',
  'settings.changePassword.current': 'Current Password',
  'settings.changePassword.new': 'New Password',
  'settings.changePassword.confirm': 'Confirm New Password',
  'settings.changePassword.submit': 'Change Password',
  'settings.changePassword.success': 'Password changed successfully.',

  // Settings - change email
  'settings.changeEmail.title': 'Change Email',
  'settings.changeEmail.current': 'Current Email',
  'settings.changeEmail.new': 'New Email',
  'settings.changeEmail.password': 'Current Password',
  'settings.changeEmail.submit': 'Send Confirmation',
  'settings.changeEmail.success': 'Confirmation sent to your new email address.',

  // Shared field labels
  'field.email': 'Email',
  'field.password': 'Password',
  'field.displayName': 'Display Name',
  'field.confirmPassword': 'Confirm Password',
  'field.companyName': 'Company Name',
  'field.contactChannel': 'Contact Channel',
  'field.customChannel': 'Custom Channel (if not listed)',
  'field.customChannelPlaceholder': 'e.g. Telegram',
  'field.contactPerson': 'Contact Person',
  'field.salaryRange': 'Salary Range',
  'field.salaryRangePlaceholder': 'e.g. $80k–$100k',
  'field.firstContactDate': 'First Contact Date',
  'field.lastContactDate': 'Last Contact Date',
  'field.vacancyPublished': 'Vacancy Published',
  'field.applicationDate': 'Application Date',
  'field.hiringStages': 'Hiring Stages',
  'field.addStagePreset': 'Add Stage (preset)',
  'field.customStage': 'Custom Stage',
  'field.customStagePlaceholder': 'Type and press Enter',
  'field.currentStage': 'Current Stage',
  'field.stageNone': '- None -',
  'field.appliedWith': 'Applied With',
  'field.appliedWithNone': '- None -',
  'field.appliedLink': 'Applied Link',
  'field.vacancyLink': 'Vacancy Link',
  'field.coverLetter': 'Cover Letter (Markdown supported)',
  'field.coverLetterPlaceholder': 'Write your cover letter here...',
  'field.vacancyText': 'Vacancy Text',
  'field.vacancyTextPlaceholder': 'Paste the job description here...',
  'field.notes': 'Notes (Markdown supported)',
  'field.notesPlaceholder': 'Your private notes...',

  // Client-side validation errors
  'validation.emailRequired': 'Email is required',
  'validation.emailInvalid': 'Enter a valid email address',
  'validation.passwordRequired': 'Password is required',
  'validation.passwordMinLength': 'At least 8 characters required',
  'validation.passwordPattern': 'Must contain at least one uppercase letter and one digit',
  'validation.displayNameRequired': 'Display name is required',
  'validation.passwordsMismatch': 'Passwords do not match',
  'validation.companyNameRequired': 'Company name is required',
  'validation.contactChannelRequired': 'Contact channel is required',
  'validation.urlInvalid': 'Must be a valid URL (https://...)',

  // List toolbar & table
  'list.search': 'Search',
  'list.searchPlaceholder': 'Company, stage, contact...',
  'list.columns': 'Columns',
  'list.addApplication': 'Add Application',
  'list.noApplications': 'No applications found.',
  'list.addFirst': 'Add your first application',

  // Table column headers
  'col.company': 'Company',
  'col.stage': 'Stage',
  'col.channel': 'Channel',
  'col.contactPerson': 'Contact Person',
  'col.applied': 'Applied',
  'col.firstContact': 'First Contact',
  'col.lastContact': 'Last Contact',
  'col.salary': 'Salary',
  'col.appliedWith': 'Applied With',
  'col.stages': 'Stages',
  'col.updated': 'Updated',
  'col.actions': 'Actions',

  // Row actions
  'action.edit': 'Edit',
  'action.delete': 'Delete',
  'action.downloadFile': 'Download file',

  // Form dialog
  'dialog.addTitle': 'Add Application',
  'dialog.editTitle': 'Edit Application',
  'dialog.cancel': 'Cancel',
  'dialog.create': 'Create',
  'dialog.saveChanges': 'Save Changes',
  'dialog.tab.basicInfo': 'Basic Info',
  'dialog.tab.dates': 'Dates',
  'dialog.tab.pipeline': 'Stages',
  'dialog.tab.notes': 'Notes & Text',
  'dialog.tab.file': 'Vacancy File',
  'dialog.addStage': 'Add stage',

  // File tab
  'file.hasAttached': 'A file is already attached.',
  'file.download': 'Download',
  'file.uploadInfo': 'Upload Vacancy File (PDF or TXT, max 10 MB)',
  'file.chooseFile': 'Choose File',

  // Column selector
  'colSelector.title': 'Manage Columns',
  'colSelector.cancel': 'Cancel',
  'colSelector.apply': 'Apply',

  // Confirm dialog
  'confirm.deleteTitle': 'Delete Application',
  'confirm.deleteMessage': 'Delete "{name}"? This cannot be undone.',
  'confirm.deleteLabel': 'Delete',

  // Settings
  'settings.title': 'Settings',
  'settings.language': 'Interface Language',
  'settings.langEn': 'English',
  'settings.langUk': 'Ukrainian',
  'settings.langRu': 'Russian',
  'settings.save': 'Save',
  'settings.back': 'Back',

  // Stage presets
  'stage.applied': 'Applied',
  'stage.screening': 'Screening',
  'stage.technicalInterview': 'Technical Interview',
  'stage.takeHomeTask': 'Take-Home Task',
  'stage.clientInterview': 'Client Interview',
  'stage.offerReceived': 'Offer Received',
  'stage.offerAccepted': 'Offer Accepted',
  'stage.rejected': 'Rejected',
  'stage.offerWithdrawn': 'Offer Withdrawn',

  // Paginator
  'paginator.itemsPerPage': 'Items per page:',
  'paginator.nextPage': 'Next page',
  'paginator.previousPage': 'Previous page',
  'paginator.firstPage': 'First page',
  'paginator.lastPage': 'Last page',
  'paginator.rangeOf': '{start} of {total}',
  'paginator.range': '{start} – {end} of {total}',

  // Snackbar messages
  'snack.deleted': 'Record deleted.',
  'snack.deleteFailed': 'Delete failed.',
  'snack.loadFailed': 'Failed to load records.',
  'snack.savedFileUploadFailed': 'Record saved but file upload failed.',
  'snack.saveFailed': 'Save failed. Please try again.',
  'snack.settingsSaved': 'Language saved.',
  'snack.settingsFailed': 'Failed to save settings.',
  'snack.sessionExpired': 'Session expired. Please sign in again.',

  // Markdown toolbar
  'md.bold': 'Bold',
  'md.italic': 'Italic',
  'md.strikethrough': 'Strikethrough',
  'md.heading': 'Heading',
  'md.list': 'List',
  'md.quote': 'Quote',
  'md.link': 'Link',
  'md.edit': 'Edit',
  'md.preview': 'Preview',
};
