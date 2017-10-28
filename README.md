# DynamicTypeCreation
# Can only be used with dynamics or Static types
         
           dynamic extended = new DynamicBuilder("WSM.Dynamic.Type", "Test", "Person")
                                  .Extend<IPerson>()
                                  .Build();

            extended.Name = "Peter";

            dynamic extended2 = new DynamicBuilder("WSM.Dynamic.Type", "Test", "Person2")
                                .Extend<IPerson>()
                                .Extend<IPersonB>()
                                .Build();
