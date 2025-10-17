-- Create the DeskProDb database
CREATE DATABASE DeskProDb;
GO

-- Use the DeskProDb database
USE DeskProDb;
GO

-- Create the Users table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Organization NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    IsAdmin BIT DEFAULT 0
);
GO

-- Create the Chamados table
CREATE TABLE Chamados (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    Descricao NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    DataCriacao DATETIME NOT NULL DEFAULT GETDATE(),
    UsuarioId INT NOT NULL,
    CONSTRAINT FK_Chamados_Users FOREIGN KEY (UsuarioId) REFERENCES Users(Id)
);
GO

-- Insert sample data for testing
INSERT INTO Users (Username, Organization, PasswordHash, Email, IsAdmin)
VALUES 
    ('admin', 'DeskPro Corp', '$2a$11$4z3X9q7y2X4z3X9q7y2X4z3X9q7y2X4z3X9q7y2X4z3X9q7y2X4', 'admin@deskpro.com', 1), -- Admin com IsAdmin = 1
    ('user1', 'Tech Solutions', '$2a$11$5y4X0r8z3Y5y4X0r8z3Y5y4X0r8z3Y5y4X0r8z3Y5y4X0r8z3Y5', 'user1@techsolutions.com', 0); -- Usuário comum com IsAdmin = 0
GO

INSERT INTO Chamados (Titulo, Descricao, Status, DataCriacao, UsuarioId)
VALUES 
    ('Problema no servidor', 'Servidor não responde', 'Aberto', GETDATE(), 1),
    ('Erro no software', 'Aplicativo trava ao iniciar', 'Em Andamento', GETDATE(), 1),
    ('Solicitação de acesso', 'Usuário precisa de permissão', 'Fechado', GETDATE(), 2);
GO