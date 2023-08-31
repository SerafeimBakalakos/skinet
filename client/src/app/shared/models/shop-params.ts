export class ShopParams {
    brandId = 0;
    typeId = 0;
    sort = 'name';
    pageIndex = 1; // better use pageNumber both here and in the API. Just stay consistent
    pageSize = 6;
    search = '';
}