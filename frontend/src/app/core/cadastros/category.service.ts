import { Injectable } from '@angular/core';
import { CrudService } from '../http/crud.service';
import {
  CategoryResponse,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from '../models/cadastros.models';

@Injectable({ providedIn: 'root' })
export class CategoryService extends CrudService<
  CategoryResponse,
  CreateCategoryRequest,
  UpdateCategoryRequest
> {
  constructor() {
    super('categories');
  }
}
