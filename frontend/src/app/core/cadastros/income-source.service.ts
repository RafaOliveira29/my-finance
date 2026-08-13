import { Injectable } from '@angular/core';
import { CrudService } from '../http/crud.service';
import {
  CreateIncomeSourceRequest,
  IncomeSourceResponse,
  UpdateIncomeSourceRequest,
} from '../models/cadastros.models';

@Injectable({ providedIn: 'root' })
export class IncomeSourceService extends CrudService<
  IncomeSourceResponse,
  CreateIncomeSourceRequest,
  UpdateIncomeSourceRequest
> {
  constructor() {
    super('income-sources');
  }
}
