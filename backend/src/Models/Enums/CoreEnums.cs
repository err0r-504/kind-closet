namespace kind_closet.Models.Enums;

public enum UserRole
{
    User = 1,
    Organizer = 2,
    Volunteer = 3,
    Moderator = 4,
    Admin = 5,
    Partner = 6
}

public enum OAuthProvider
{
    None = 0,
    Vk = 1,
    Yandex = 2
}

public enum PostType
{
    Give = 1,    // Отдам
    Seek = 2,    // Ищу
    Lend = 3,    // Одолжу
    Borrow = 4   // Возьму на время
}

public enum ItemCondition
{
    New = 1,       // Новое
    Excellent = 2, // Отличное
    Good = 3       // Хорошее
}

public enum ExchangeFormat
{
    Free = 1,      // Бесплатно
    Points = 2,    // По балльной системе
    Charity = 3    // На благотворительной основе
}

public enum ListingStatus
{
    Draft = 1,
    Published = 2,
    Reserved = 3,
    Transferred = 4,
    Completed = 5,
    Cancelled = 6
}

public enum PointStatus
{
    Active = 1,
    Paused = 2,
    Closed = 3
}

public enum TransferStatus
{
    Agreed = 1,      // Забронировано / Договорились
    Transferred = 2, // Передано (одна сторона подтвердила)
    Completed = 3,   // Завершено (двустороннее подтверждение)
    Cancelled = 4    // Отменено
}