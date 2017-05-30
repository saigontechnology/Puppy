#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TestTypeEnum.cs </Name>
//         <Created> 30/05/2017 4:41:54 PM </Created>
//         <Key> 0729c372-893a-4256-9c39-ae384759e8be </Key>
//     </File>
//     <Summary>
//         TestTypeEnum.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.OneSignal.Devices
{
    /// <summary>
    ///     Test type enumeration. 
    /// </summary>
    public enum TestTypeEnum
    {
        /// <summary>
        ///     Used during development phase. 
        /// </summary>
        Development = 1,

        /// <summary>
        ///     Used in production, when trying to track down undelivered messages for example. 
        /// </summary>
        AdHoc = 2
    }
}