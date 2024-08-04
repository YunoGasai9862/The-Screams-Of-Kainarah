using System;
public interface IAssetType<T>: IAssetTypeAbstract
{
   new public abstract Type GetAssetType();
}