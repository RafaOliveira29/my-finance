import { Injectable } from '@angular/core';
import { CrudService } from '../http/crud.service';
import {
  CreateExpenseSourceRequest,
  ExpenseSourceResponse,
  UpdateExpenseSourceRequest,
} from '../models/cadastros.models';

@Injectable({ providedIn: 'root' })
export class ExpenseSourceService extends CrudService<
  ExpenseSourceResponse,
  CreateExpenseSourceRequest,
  UpdateExpenseSourceRequest
> {
  constructor() {
    super('expense-sources');
  }
}
