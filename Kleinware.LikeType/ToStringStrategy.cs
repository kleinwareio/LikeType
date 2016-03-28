namespace Kleinware.LikeType
{
    public enum ToStringStrategy
    {
        /// <summary>
        /// ToString will only display the name of the type and the number of elements in the collection.
        /// 
        /// Will look similar to: MyType[4]
        /// </summary>
        CountOnly,
        /// <summary>
        /// ToString will display the name of the type, how many elements there are, and the ToString value of each element, on a single line.
        /// 
        /// Will look similar to: MyType[2] = { '14', '22' }
        /// </summary>
        AllValuesSingleLine,
        /// <summary>
        /// ToString will display the name of the type, how many elements there are, and the ToString value of each element, with each item on its own line.
        /// 
        /// Will look similar to:
        /// MyType[2] = {
        /// \r\n  '14',
        /// \r\n  '22' }
        /// </summary>
        AllValuesMultiLine
    }
}