# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

This project content Entities, Models and Interfaces only without any business logic, implementation or Helper method.

1. Service Folder
- Include all Interface of services
- Implementation in TopCore.Auth.Service

2. Data Folder
- Include all Interface of data (repository)
- Implementation in TopCore.Auth.Data

3. Models Folder
- Included all ViewModel, Model shared between TopCore.Auth and TopCore.Auth.Service
- It will included validation basic by annotation or another validate framework.

4. Entities Folder
- Included all Entity (Object for Database)
- Use in TopCore.Auth.Data
- All Entity must inheritance from EntityBase abstract class
- Mapping (convert entity class to Entity Framework class in Mapping folder of TopCore.Auth.Data Project)
- Can use only System.ComponentModel.DataAnnotations for define DataType, Key and so on.
- Framework Implement need follow the convention like Entity Framework