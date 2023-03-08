﻿import { Injectable, Injector } from '@angular/core';
import { ArticleLocalization, ArticleModel, ErrorDto, LocalizationsApi } from 'src/app/api';
import { ISelectOption } from 'src/app/components/form/types';
import { MetadataService } from 'src/app/services';
import { FormServiceBase } from 'src/app/services/abstractions';

@Injectable()
export class LocalizationService extends FormServiceBase {
  contentStatuses: ISelectOption[] = [];

  constructor(
    injector: Injector,
    private readonly metadataService: MetadataService,
    private readonly localizationsApi: LocalizationsApi,
  ) {
    super(injector, {
      id: ['', []],
      articleId: ['', []],
      languageCode: ['', []],
      contributors: ['', []],
      title: ['', []],
      description: ['', []],
      html: ['', []],
      status: ['', []],
      views: ['', []],
      rate: ['', []],
      created: ['', []],
      updated: ['', []],
      published: ['', []],
      banned: ['', []],
      isSaved: ['', []],
      savedDate: ['', []],
      vote: ['', []],
    });
  }

  loadMetadata() {
    function getMarkerColorByStatus(statusKey: string): string {
      switch (statusKey.toLowerCase()) {
        case 'published':
          return 'var(--c-success)';
        case 'banned':
          return 'var(--c-danger)';
        default:
          return 'var(--c-bg-1)';
      }
    }

    this.setReady('loading');
    this.metadataService.getContentStatuses().subscribe(result => {
      this.contentStatuses = result.map(x => ({
        ...x,
        markerColor: getMarkerColorByStatus(x.key),
      }));
      this.setReady('ready');
    });
  }

  // getById(id: number): void {
  //   this.onRequestStart();
  //   this.localizationsApi.getById(id).subscribe({
  //     next: (localization: ArticleLocalizationModel | any) => {
  //       localization.ready = true;
  //       this.onRequestSuccess();
  //     },
  //     error: (error: ErrorDto) => {
  //       this.onRequestError(error);
  //     },
  //   });
  // }

  update(id: number): void {
    const request = this.form.value as ArticleLocalization;
    request.id = id;
    this.onRequestStart();
    this.localizationsApi.update(request).subscribe({
      next: () => {
        this.toaster.showSuccess('Localization successfully updated');
        this.onRequestSuccess();
      },
      error: (error: ErrorDto) => {
        this.onRequestError(error);
      },
    });
  }

  delete(id: number): void {
    this.modals.showConfirmDelete({
      confirmed: () => {
        this.onRequestStart();
        this.localizationsApi.delete(id).subscribe({
          next: () => {
            this.toaster.showSuccess('Localization successfully deleted');
            this.router.navigateByUrl('articles');
            this.onRequestSuccess();
          },
          error: (error: ErrorDto) => {
            this.onRequestError(error);
          },
        });
      },
    });
  }

  private onRequestStart() {
    // console.log('request started');
  }

  private onRequestSuccess() {
    // console.log('request succeed');
  }

  private onRequestError(error: ErrorDto) {
    // console.log('request error');
    this.toaster.showFail(error.message);
  }
}
