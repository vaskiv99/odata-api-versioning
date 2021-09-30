using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.Examples
{
    /// <summary>
    /// Lightweight replacement of <see cref="ODataQueryOptions"/>. Exposes unwrapped query option values.
    /// </summary>
    /// <remarks>
    /// When used it is required to specify binding source explicitly.
    /// </remarks>
    /// <typeparam name="TElement">Element type of the entity set being queried.</typeparam>
    [ModelBinder( typeof( ODataQueryOptionsSlimModelBinder ) )]
    public sealed class ODataQueryOptionsSlim<TElement> : IValidatableObject
        where TElement : class
    {
        internal static ODataQueryOptionsSlim<TElement> Empty { get; }
            = new ODataQueryOptionsSlim<TElement>();

        private readonly FilterQueryOption? _filterOption;
        private readonly OrderByQueryOption? _orderByOption;
        private readonly SelectExpandQueryOption? _selectExpandOption;
        private readonly CountQueryOption? _countOption;
        private readonly SkipQueryOption? _skipOption;
        private readonly TopQueryOption? _topOption;

        private readonly ODataValidationSettings _validationSettings;

        internal ODataQueryOptionsSlim(
            FilterQueryOption? filter,
            OrderByQueryOption? orderBy,
            SelectExpandQueryOption? selectExpand,
            CountQueryOption? count,
            SkipQueryOption? skip,
            TopQueryOption? top,
            ODataValidationSettings validationSettings )
        {
            _filterOption = filter;
            _orderByOption = orderBy;
            _selectExpandOption = selectExpand;
            _countOption = count;
            _skipOption = skip;
            _topOption = top;

            _validationSettings = validationSettings;
        }

        private ODataQueryOptionsSlim()
        {
            _validationSettings = new ODataValidationSettings();
        }

        [ValidateNever]
        public FilterClause? Filter => _filterOption?.FilterClause;

        [ValidateNever]
        public OrderByClause? OrderBy => _orderByOption?.OrderByClause;

        [ValidateNever]
        public SelectExpandClause? SelectExpand => _selectExpandOption?.SelectExpandClause;

        [ValidateNever]
        public bool Count => _countOption?.Value ?? false;

        [ValidateNever]
        public int? Skip => _skipOption?.Value;

        [ValidateNever]
        public int? Top => _topOption?.Value;

        public ODataQueryOptionsSlim<TElement> WithSelect( SelectExpandQueryOption selectExpandQueryOption )
        {
            return new ODataQueryOptionsSlim<TElement>(
                _filterOption,
                _orderByOption,
                selectExpandQueryOption,
                _countOption,
                _skipOption,
                _topOption,
                _validationSettings );
        }

        public IEnumerable<ValidationResult> Validate( ValidationContext validationContext )
        {
            yield return ValidateODataProperty( ODataQueryOptionName.Filter, ValidateFilter );
            yield return ValidateODataProperty( ODataQueryOptionName.OrderBy, ValidateOrderBy );
            yield return ValidateODataProperty( ODataQueryOptionName.Select + ODataQueryOptionName.Expand, ValidateSelectExpand );
            yield return ValidateODataProperty( ODataQueryOptionName.Count, ValidateCount );
            yield return ValidateODataProperty( ODataQueryOptionName.Skip, ValidateSkip );
            yield return ValidateODataProperty( ODataQueryOptionName.Top, ValidateTop );
        }

        private ValidationResult ValidateODataProperty(
            ODataQueryOptionName optionName,
            Action validationAction )
        {
            try
            {
                validationAction();

                return ValidationResult.Success;
            }
            catch ( ODataException oDataException )
            {
                var memberNames = new string[] { optionName };
                return new ValidationResult( oDataException.Message, memberNames );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Discards are used in Validate methods, because some validators don't ever touch the actual value and pass.
        // While the value getter throws!
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ValidateFilter()
        {
            if ( _filterOption != null )
            {
                _filterOption.Validate( _validationSettings );
                _ = _filterOption.FilterClause;
            }
        }

        private void ValidateOrderBy()
        {
            if ( _orderByOption != null )
            {
                _orderByOption.Validate( _validationSettings );
                _ = _orderByOption.OrderByClause;
            }
        }

        private void ValidateSelectExpand()
        {
            if ( _selectExpandOption != null )
            {
                _selectExpandOption.Validate( _validationSettings );
                _ = _selectExpandOption.SelectExpandClause;
            }
        }

        private void ValidateCount()
        {
            if ( _countOption != null )
            {
                _countOption.Validate( _validationSettings );
                _ = _countOption.Value;
            }
        }

        private void ValidateSkip()
        {
            if ( _skipOption != null )
            {
                _skipOption.Validate( _validationSettings );
                _ = _skipOption.Value;
            }
        }

        private void ValidateTop()
        {
            if ( _topOption != null )
            {
                _topOption.Validate( _validationSettings );
                _ = _topOption.Value;
            }
        }
    }
}