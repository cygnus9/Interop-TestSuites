namespace Microsoft.Protocols.TestSuites.MS_OXWSMSG
{
    using Microsoft.Protocols.TestSuites.Common;
    using Microsoft.Protocols.TestTools;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This scenario is designed to test operation related to copy of an email message on the server.
    /// </summary>
    [TestClass]
    public class S03_CopyEmailMessage : TestSuiteBase
    {
        #region Fields
        /// <summary>
        /// The first Item of the first responseMessageItem in infoItems returned from server response.
        /// </summary>
        private ItemType firstItemOfFirstInfoItem;

        /// <summary>
        /// The related Item of ItemInfoResponseMessageType returned from server.
        /// </summary>
        private ItemInfoResponseMessageType[] infoItems;
        #endregion

        #region Class initialize and clean up
        /// <summary>
        /// Initialize the test class.
        /// </summary>
        /// <param name="context">Context to initialize.</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TestClassBase.Initialize(context);
        }

        /// <summary>
        /// Clean up the test class.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            TestClassBase.Cleanup();
        }
        #endregion

        #region Test cases
        /// <summary>
        /// This test case is used to verify the related requirements about the server behavior when copying E-mail message.
        /// </summary>
        [TestCategory("MSOXWSMSG"), TestMethod()]
        public void MSOXWSMSG_S03_TC01_CopyMessage()
        {
            #region Create message
            CreateItemType createItemRequest = GetCreateItemType(MessageDispositionType.SaveOnly, DistinguishedFolderIdNameType.drafts);
            CreateItemResponseType createItemResponse = this.MSGAdapter.CreateItem(createItemRequest);
            Site.Assert.IsTrue(this.VerifyCreateItemResponse(createItemResponse, MessageDispositionType.SaveOnly), @"Server should return success for creating the email messages.");
            this.infoItems = TestSuiteHelper.GetInfoItemsInResponse(createItemResponse);
            this.firstItemOfFirstInfoItem = TestSuiteHelper.GetItemTypeItemFromInfoItemsByIndex(this.infoItems, 0, 0);

            // Save the ItemId of message responseMessageItem got from the createItem response.
            ItemIdType itemIdType = new ItemIdType();
            Site.Assert.IsNotNull(this.firstItemOfFirstInfoItem.ItemId, @"The ItemId property of the first item should not be null.");
            itemIdType.Id = this.firstItemOfFirstInfoItem.ItemId.Id;
            itemIdType.ChangeKey = this.firstItemOfFirstInfoItem.ItemId.ChangeKey;
            #endregion

            #region Copy message
            CopyItemType copyItemRequest = new CopyItemType
            {
                ItemIds = new ItemIdType[]
                {
                    itemIdType
                },

                // Save the copy message to inbox folder.
                ToFolderId = new TargetFolderIdType
                {
                    Item = new DistinguishedFolderIdType
                    {
                        Id = DistinguishedFolderIdNameType.inbox
                    }
                }
            };

            CopyItemResponseType copyItemResponse = this.MSGAdapter.CopyItem(copyItemRequest);
            Site.Assert.IsTrue(this.VerifyResponse(copyItemResponse), @"Server should return success for copying the email messages.");
            this.infoItems = TestSuiteHelper.GetInfoItemsInResponse(copyItemResponse);
            this.firstItemOfFirstInfoItem = TestSuiteHelper.GetItemTypeItemFromInfoItemsByIndex(this.infoItems, 0, 0);
            Site.Assert.IsNotNull(this.infoItems, @"InfoItems in the returned response should not be null.");
            Site.Assert.IsNotNull(this.firstItemOfFirstInfoItem, @"The first item of the array of ItemType type from server response should not be null.");

            // Save the ItemId of message responseMessageItem got from the copyItem response.
            ItemIdType copyItemIdType = new ItemIdType();
            Site.Assert.IsNotNull(this.firstItemOfFirstInfoItem.ItemId, @"The ItemId property of the first item should not be null.");
            copyItemIdType.Id = this.firstItemOfFirstInfoItem.ItemId.Id;
            copyItemIdType.ChangeKey = this.firstItemOfFirstInfoItem.ItemId.ChangeKey;

            #region Verify the requirements about CopyItem
            // Add the debug information
            Site.Log.Add(LogEntryKind.Debug, "Verify MS-OXWSMSG_R168");
        
            // Verify MS-OXWSMSG requirement: MS-OXWSMSG_R168            
            Site.CaptureRequirementIfIsNotNull(
                copyItemResponse,
                168,
                @"[In CopyItem] The protocol client sends a CopyItemSoapIn request WSDL message, and the protocol server responds with a CopyItemSoapOut response WSDL message.");

            Site.Assert.IsNotNull(this.infoItems[0].ResponseClass, @"The ResponseClass property of the first item of infoItems instance should not be null.");
            
            // Add the debug information
            Site.Log.Add(LogEntryKind.Debug, "Verify MS-OXWSMSG_R169");
        
            // Verify MS-OXWSMSG requirement: MS-OXWSMSG_R169            
            Site.CaptureRequirementIfAreEqual<ResponseClassType>(
                ResponseClassType.Success,
                copyItemResponse.ResponseMessages.Items[0].ResponseClass,
                169,
                @"[In CopyItem] If the CreateItem WSDL operation request is successful, the server returns a CreateItemResponse element, as specified in [MS-OXWSCORE] section 3.1.4.2.2.2, with the ResponseClass attribute, as specified in [MS-OXWSCDATA] section 2.2.4.67, of the CreateItemResponseMessage element, as specified in [MS-OXWSCDATA] section 2.2.4.12, set to ""Success"". ");

            Site.Assert.IsNotNull(this.infoItems[0].ResponseCode, @"The ResponseCode property of the first item of infoItems instance should not be null.");
            
            // Add the debug information
            Site.Log.Add(LogEntryKind.Debug, "Verify MS-OXWSMSG_R170");
        
            // Verify MS-OXWSMSG requirement: MS-OXWSMSG_R170
            Site.CaptureRequirementIfAreEqual<ResponseCodeType>(
                ResponseCodeType.NoError,
                copyItemResponse.ResponseMessages.Items[0].ResponseCode,
                170,
                @"[In CopyItem] [A successful CopyItem operation request returns a CopyItemResponse element] The ResponseCode element, as specified in [MS-OXWSCDATA] section 2.2.4.67, of the CreateItemResponseMessage element is set to ""NoError"". ");
            #endregion
            #endregion

            #region Delete the copied Email messages
            DeleteItemType deleteItemRequest = new DeleteItemType
            {
                ItemIds = new ItemIdType[]
                {
                   itemIdType,
                   copyItemIdType
                }
            };

            DeleteItemResponseType deleteItemResponse = this.MSGAdapter.DeleteItem(deleteItemRequest);
            Site.Assert.IsTrue(this.VerifyMultipleResponse(deleteItemResponse), @"Server should return success for deleting the email messages.");
            #endregion
        }

        /// <summary>
        /// This test case is used to verify the related requirements about the server behavior when copying E-mail message unsuccessful.
        /// </summary>
        [TestCategory("MSOXWSMSG"), TestMethod()]
        public void MSOXWSMSG_S03_TC02_CopyMessageUnsuccessful()
        {
            #region Create message
            CreateItemType createItemRequest = GetCreateItemType(MessageDispositionType.SaveOnly, DistinguishedFolderIdNameType.drafts);
            CreateItemResponseType createItemResponse = this.MSGAdapter.CreateItem(createItemRequest);
            Site.Assert.IsTrue(this.VerifyCreateItemResponse(createItemResponse, MessageDispositionType.SaveOnly), @"Server should return success for creating the email messages.");
            this.infoItems = TestSuiteHelper.GetInfoItemsInResponse(createItemResponse);
            this.firstItemOfFirstInfoItem = TestSuiteHelper.GetItemTypeItemFromInfoItemsByIndex(this.infoItems, 0, 0);

            // Save the ItemId of message responseMessageItem got from the createItem response.
            ItemIdType itemIdType = new ItemIdType();
            Site.Assert.IsNotNull(this.firstItemOfFirstInfoItem.ItemId, @"The ItemId property of the first item should not be null.");
            itemIdType.Id = this.firstItemOfFirstInfoItem.ItemId.Id;
            itemIdType.ChangeKey = this.firstItemOfFirstInfoItem.ItemId.ChangeKey;
            #endregion

            #region Delete the message created
            DeleteItemType deleteItemRequest = new DeleteItemType
            {
                ItemIds = new ItemIdType[]
                {
                   this.firstItemOfFirstInfoItem.ItemId
                }
            };

            DeleteItemResponseType deleteItemResponse = this.MSGAdapter.DeleteItem(deleteItemRequest);
            Site.Assert.IsTrue(this.VerifyResponse(deleteItemResponse), @"Server should return success for deleting the email messages.");
            #endregion

            #region Copy message deleted
            CopyItemType copyItemRequest = new CopyItemType
            {
                ItemIds = new ItemIdType[]
                {
                    itemIdType
                },

                // Save the copy message to inbox folder.
                ToFolderId = new TargetFolderIdType
                {
                    Item = new DistinguishedFolderIdType
                    {
                        Id = DistinguishedFolderIdNameType.inbox
                    }
                }
            };

            CopyItemResponseType copyItemResponse = this.MSGAdapter.CopyItem(copyItemRequest);

            // Add the debug information
            Site.Log.Add(LogEntryKind.Debug, "Verify MS-OXWSMSG_R170001");

            this.Site.CaptureRequirementIfAreEqual<ResponseClassType>(
                ResponseClassType.Error,
                copyItemResponse.ResponseMessages.Items[0].ResponseClass,
                170001,
                @"[In CopyItem] If the CreateItem WSDL operation is not successful, it returns a CreateItemResponse element with the ResponseClass attribute of the CreateItemResponseMessage element set to ""Error"". ");

            // Add the debug information
            Site.Log.Add(LogEntryKind.Debug, "Verify MS-OXWSMSG_R170002");

            this.Site.CaptureRequirementIfIsTrue(
                System.Enum.IsDefined(typeof(ResponseCodeType), copyItemResponse.ResponseMessages.Items[0].ResponseCode),
                170002,
                @"[In CopyItem] [A unsuccessful CopyItem operation request returns a CopyItemResponse element] The ResponseCode element of the CreateItemResponseMessage element is set to one of the common errors defined in [MS-OXWSCDATA] section 2.2.5.24.");
            #endregion
        }
        #endregion
    }
}