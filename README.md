# Reveries 💫
> Developed by [Anders Lynge Ravnsbæk](https://www.linkedin.com/in/alravnsbaek/)

Reveries is a personal hobby project that I am developing in my spare time alongside job hunting. The project serves both as a way to maintain and expand my coding skills and as an opportunity to experiment with new technologies, best practices, integrations, and design patterns that can strengthen my skills and competencies as a developer.  

It is therefore both a learning project and a product I personally want to use to support my passion for reading books and one I look forward to putting into use.  

### Vision 🔭
Reveries aims to be a digital book hub where you can build and organize your own collection in the form of a virtual bookshelf.  

The goal is to create a platform that makes it easier to get an overview of the books you own, provides insights into your reading habits, and presents metadata and statistics about which books you have read and which are still waiting to be explored - and who knows what future features will be implemented.  

The project is being developed as a fullstack application, hosted on a Raspberry Pi and accessible through my private network. This makes it a complete learning environment, covering everything from backend and database to frontend and deployment.  

### Bookscanner
For easier creation of books in the system, a book scanner has been made to make it faster and easier to add all the books in your shelf. The scanner scans the ISBN barcode on the book, and the backend quickly retrieves the book metadate from the external APIs.


<img width="393" height="1003" alt="Screenshot 2025-11-18 at 20-53-07 Reveries The Will Of The Many" src="https://github.com/user-attachments/assets/71586414-ee14-4308-9226-9cfe197df0b8" />
<img width="393" height="1584" alt="Screenshot 2025-11-18 at 20-52-24 Reveries The Will Of The Many" src="https://github.com/user-attachments/assets/54ef04cb-632d-4df3-9463-936ede42ac8d" />


### Project Plan 🎯
The plan for the project is outlined below. The choice of technologies is primarily driven by the desire to gain hands-on experience with widely used and relevant tools from the real world, not necessarily because they represent the most optimal solution for this particular application.  

- **Backend**
  - [x] **C# / .NET API**  
    The business logic is developed with a focus on following SOLID principles and Clean Architecture to ensure scalable and maintainable code.  
  - [x] **Dapper**  
    Used as a micro-ORM instead of EF-Core to gain more control over SQL and create a closer connection between code and database.  
  - [x] **External APIs - ISBNDB and Google Books**  
    Integration with external APIs that provides book data, allowing new titles to be added quickly with rich metadata.  

- **Frontend**
  - [ ] **TypeScript and React**  
    A modern and responsive interface built with React to gain practical experience with one of the most widely adopted frontend libraries.  

- **Database**
  - [x] **PostgreSQL**  
    Chosen to build experience with one of the most popular relational databases, also widely used in large-scale projects.  

- **Cache**
  - [x] **Redis**  
    Implemented to optimize response times and provide a faster, more responsive user experience. Using a Cache-Aside strategy, data is cached in memory and refreshed from the database when needed.

- **CI/CD**
  - [ ] **Docker and GitHub Actions**  
    Pipelines automatically package new builds into Docker images and deploy them to the Raspberry Pi, ensuring the application always runs the latest version.  

### Technologies 🚀
- [C# / .NET](https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/)  
- [Dapper](https://www.learndapper.com/)
- [ISBNDB API](https://isbndb.com/)
- [Google Books API](https://developers.google.com/books)
- TypeScript / React  
- [PostgreSQL](https://www.postgresql.org/docs/)  
- [Redis and RedisInsight](https://redis.io/docs/latest/)
- [Docker](https://docs.docker.com/)  
- GitHub Actions  

### Database Schema 📋
Below is an ER diagram of the current entities in the project.  
The diagram provides an overview of the tables, their relationships, and their fields. It serves as the foundation for the database schema implementation and helps keep the structure organized as the project grows.  
  

```mermaid
erDiagram
    authors {
        int id PK
        varchar normalized_name UK
        varchar first_name
        varchar last_name
        timestamp date_created
    }
    
    author_name_variants {
        int id PK
        int author_id FK
        varchar name_variant
        boolean is_primary
        timestamp date_added
    }
    
    books {
        int id PK
        varchar title
        varchar isbn13 UK
        varchar isbn10 UK
        int publisher_id FK
        int series_id FK
        int series_number
        date publication_date
        int page_count
        text synopsis
        varchar language
        varchar language_iso639
        varchar edition
        varchar binding
        text image_url
        text image_thumbnail
        decimal msrp
        boolean is_read
        timestamp date_created
    }
    
    publishers {
        int id PK
        varchar name UK
        timestamp date_created
    }
    
    subjects {
        int id PK
        varchar name UK
        timestamp date_created
    }
    
    book_dimensions {
        int book_id PK, FK
        decimal height_cm
        decimal width_cm
        decimal thickness_cm
        decimal weight_g
        timestamp date_created
    }
    
    books_authors {
        int book_id PK, FK
        int author_id PK, FK
        timestamp date_created
    }
    
    books_subjects {
        int book_id PK, FK
        int subject_id PK, FK
        timestamp date_created
    }
    
    dewey_decimals {
        int book_id PK, FK
        varchar code PK
        timestamp date_created
    }

    series {
        int series_id PK
        varchar name UK
        timestamp date_created
    }

    authors ||--o{ author_name_variants : "has"
    authors ||--o{ books_authors : "writes"
    books ||--o{ books_authors : "written by"
    books ||--o{ books_subjects : "categorized as"
    subjects ||--o{ books_subjects : "categorizes"
    publishers ||--o{ books : "publishes"
    books ||--|| book_dimensions : "has"
    books ||--o{ dewey_decimals : "has"
    series ||--o{ books : "contains"
```
