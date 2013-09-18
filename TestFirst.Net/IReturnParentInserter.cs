namespace TestFirst.Net
{
    /// <summary>
    /// Returns the parent inserter to allow chaining of builders. 
    /// 
    /// E.g.
    /// 
    /// .Given(Foo.With()
    ///     .Bar(1).BarAttribute()
    ///     .Bar(2).BarAttribute() ) //bar builder here but we want to call Foo.Insert instead
    /// 
    /// where the 'Bar' builder implements this interface and returns the 'Foo' builder rather than have an additional method
    /// needing to be called on Bar by the user. 
    /// 
    /// The scenario will walk up the parent inserter chain until it gets to the top and calls the root inserter
    /// 
    /// </summary>
    public interface IReturnParentInserter
    {

        IInserter GetParentInserter();
    }
}
