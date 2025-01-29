CREATE TABLE public."UserInformation" (
	"Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "UserName" TEXT NOT NULL,
    "ShowTopics" BOOLEAN NOT NULL,
    "ShowComments" BOOLEAN NOT NULL,
    "ProfilePicture" TEXT
) TABLESPACE pg_default;