using FlexiForms;
using FlexiForms.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Actions;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.ModelBinders;

namespace FlexiForms.Backoffice.Section
{
    [Tree(
        FlexiFormConstants.Backoffice.SectionAlias,
        treeAlias: FlexiFormConstants.Backoffice.TreeAlias,
        TreeTitle = FlexiFormConstants.Backoffice.TreeName,
        TreeGroup = Constants.Trees.Groups.ThirdParty,
        SortOrder = 12
    )]
    [PluginController(FlexiFormConstants.Application.PluginName)]
    public class FlexiFormsTreeController : TreeController
    {
        private readonly IMenuItemCollectionFactory _menuItemsFactory;
        private readonly IFlexiFormsResponsesRepository _responseRepository;

        public FlexiFormsTreeController(
            ILocalizedTextService localizedTextService, 
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, 
            IEventAggregator eventAggregator,
            IMenuItemCollectionFactory menuItemCollectionFactory,
            IFlexiFormsResponsesRepository responseRepository
        ) : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _menuItemsFactory = menuItemCollectionFactory;
            _responseRepository = responseRepository;
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, FormCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            var groupByIdentifier = _responseRepository.GetAliases().Result;

            if (id == Constants.System.Root.ToInvariantString())
            {
                int count = 0;
                foreach (var identifier in groupByIdentifier)
                {
                    var node = CreateTreeNode(
                        count.ToString(), 
                        "-1", 
                        queryStrings, 
                        identifier.Identifier, 
                        "icon-chart-curve", 
                        false,
                        $"{FlexiFormConstants.Backoffice.TreeUrl}/view/{identifier.Identifier}"
                    );
                    nodes.Add(node);
                    count++;
                }
            }

            return nodes;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, FormCollection queryStrings)
        {
            var menu = _menuItemsFactory.Create();

            if (id == Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add(new CreateChildEntity(LocalizedTextService));
                menu.Items.Add(new RefreshNode(LocalizedTextService, true));
            }
            else
            {
                menu.Items.Add<ActionDelete>(LocalizedTextService, true, opensDialog: true);
            }

            return menu;
        }

        protected override ActionResult<TreeNode?> CreateRootNode(FormCollection queryStrings)
        {
            var rootResult = base.CreateRootNode(queryStrings);

            if (rootResult.Result is not null)
            {
                return rootResult;
            }

            var root = rootResult.Value;

            if (root is not null)
            {
                root.RoutePath = FlexiFormConstants.Backoffice.TreeRootUrl;
                root.Icon = "icon-chart-curve";
                root.HasChildren = true;
                root.MenuUrl = null;
            }

            return root;
        }

    }
}
