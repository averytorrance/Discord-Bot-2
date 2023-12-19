/*   USER PROFILES   */
CREATE TABLE UserDim(
    UserID nvarchar(50) NOT NULL PRIMARY KEY,
	Username nvarchar(50) NOT NULL,
	isDuplicateUser bit NULL,
	OriginalUserID nvarchar(50) NULL,
	Name nvarchar(50) NULL,
	Address nvarchar(50) NULL,
	Timezone nvarchar(50) NULL,
);

/*   SERVER   */
CREATE TABLE Errors(
	ID int NOT NULL PRIMARY KEY,
	Instant datetime2(7) NOT NULL,
	Error nvarchar(50) NOT NULL,
	Command nvarchar(MAX) NULL,
	ServerID nvarchar(50) NULL
);

CREATE TABLE CommandLog(
	Instant datetime2(7) NOT NULL,
	Module int NOT NULL FOREIGN KEY REFERENCES ModuleDim(ModuleID),
	UserID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID),
	ErrorID int NULL FOREIGN KEY REFERENCES Errors(ID),
	ServerID nvarchar(50) NULL,
	Command nvarchar(50) NULL,
);

CREATE TABLE EventLog(
	Instant datetime2(7) NOT NULL,
	Module int NOT NULL FOREIGN KEY REFERENCES ModuleDim(ModuleID),
	UserID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID),
	ErrorID int NULL FOREIGN KEY REFERENCES Errors(ID),
	ServerID nvarchar(50) NULL,
	Command nvarchar(50) NULL,
	Runtime int NULL,
);

/*   WATCHRATINGS   */
CREATE TABLE GenreDim (
	ID int NOT NULL PRIMARY KEY,
	Genre nvarchar(50) NOT NULL
);

CREATE TABLE CountryDim (
	ID int NOT NULL Primary KEY,
	Country nvarchar(50) NOT NULL
);

CREATE TABLE LanguageDim (
	ID int NOT NULL PRIMARY KEY,
	Language nvarchar(50) NOT NULL
);

CREATE TABLE WatchDim (
    ID nvarchar(50) NOT NULL PRIMARY KEY,
    Title nvarchar(254) NOT NULL,
    Year int NULL,
	Type nvarchar(50) NULL,
	isRewatch bit NOT NULL,
	Instant datetime2(7) NOT NULL,
	GenreID int  NULL FOREIGN KEY REFERENCES GenreDim(ID),
	CountryID int  NULL FOREIGN KEY REFERENCES CountryDim(ID),
	LanguageID int  NULL FOREIGN KEY REFERENCES LanguageDim(ID)
);

CREATE TABLE RatingsFact (
	WatchID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES WatchDim(ID),
	UserID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID),
	Score float NOT NULL
);

CREATE TABLE WatchSuggestionDim (
    ID nvarchar(50) NOT NULL PRIMARY KEY,
    Title nvarchar(50) NOT NULL,
	UserID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID)
);

/*   REMINDERS   */
CREATE TABLE ReminderFact(
	ReminderID int NOT NULL PRIMARY KEY,
	Message nvarchar(max) NOT NULL,
	OwnerId nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID),
	Sent bit NOT NULL,
	SendInstant datetime2(7) NOT NULL,
	Recurring bit NOT NULL, 
	Frequency nvarchar(50) NOT NULL,
	DayOfWeek nvarchar(50) NOT NULL,
	SoftDeleted bit NOT NULL
);

CREATE TABLE SubscriptionsDim(
	SubscriptionID nvarchar(50) NOT NULL PRIMARY KEY,
	UserID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID),
	ReminderID int NOT NULL FOREIGN KEY REFERENCES	ReminderFact(ReminderID),
);

/*   SECRET SANTA  */
CREATE TABLE SecretSantaSessionsFact(
	SessionYear int NOT NULL PRIMARY KEY,
	StartDate datetime2(7) NOT NULL,
	CloseDate datetime2(7) NOT NULL,
);

CREATE TABLE PresentFact(
	ID int NOT NULL PRIMARY KEY,
	Present nvarchar(50) NULL,
	isDelivered bit NOT NULL,
	TrackingNumber nvarchar(50) NULL
);

CREATE TABLE SecretSantaUserDim(
	SessionYear int NOT NULL FOREIGN KEY REFERENCES SecretSantaSessionFact(SessionYear),
	UserID nvarchar(50) NOT NULL FOREIGN KEY REFERENCES UserDim(UserID),
	isParticipant bit NOT NULL,
	SantaID nvarchar(50) NULL FOREIGN KEY REFERENCES UserDim(UserID),
	PresentID int NOT NULL FOREIGN KEY REFERENCES PRESENTSFACT(ID)
);

